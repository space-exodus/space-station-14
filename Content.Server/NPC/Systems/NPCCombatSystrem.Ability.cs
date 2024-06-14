using System.Numerics;
using Content.Server.NPC.Components;
using Content.Shared.NPC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Random;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions.Events;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;

using Content.Shared.Interaction;
using Content.Shared.Actions;
using Content.Server.Actions;
using Content.Shared.Directions;

namespace Content.Server.NPC.Systems;

public sealed partial class NPCCombatSystem
{
    [Dependency] private readonly ActionsSystem _actions = default!;
    [Dependency] private readonly ActionBlockerSystem _actionBlockerSystem = default!;
    [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;
    [Dependency] private readonly RotateToFaceSystem _rotateToFaceSystem = default!;

    private const float TargetAbilityLostRange = 28f;

    private void InitializeAbility()
    {
        SubscribeLocalEvent<NPCAbilityCombatComponent, ComponentShutdown>(OnAbilityShutdown);
    }

    private void OnAbilityShutdown(EntityUid uid, NPCAbilityCombatComponent component, ComponentShutdown args)
    {
        _steering.Unregister(uid);
    }

    private void UpdateAbility(float frameTime)
    {
        var xformQuery = GetEntityQuery<TransformComponent>();
        var physicsQuery = GetEntityQuery<PhysicsComponent>();
        var curTime = _timing.CurTime;
        var query = EntityQueryEnumerator<NPCAbilityCombatComponent, ActiveNPCComponent>();

        while (query.MoveNext(out var uid, out var comp, out _))
        {
            CastAction(uid, comp, curTime, physicsQuery, xformQuery);
        }
    }

    private void CastAction(EntityUid uid, NPCAbilityCombatComponent combatComp, TimeSpan curTime, EntityQuery<PhysicsComponent> physicsQuery, EntityQuery<TransformComponent> xformQuery)
    {
        combatComp.Status = AbilityCombatStatus.Normal;

        if (!xformQuery.TryGetComponent(uid, out var xform) ||
            !xformQuery.TryGetComponent(combatComp.Target, out var targetXform))
        {
            combatComp.Status = AbilityCombatStatus.TargetUnreachable;
            return;
        }

        if (!xform.Coordinates.TryDistance(EntityManager, targetXform.Coordinates, out var distance))
        {
            combatComp.Status = AbilityCombatStatus.TargetUnreachable;
            return;
        }

        if (distance > TargetMeleeLostRange)
        {
            combatComp.Status = AbilityCombatStatus.TargetUnreachable;
            return;
        }

        if (TryComp<NPCSteeringComponent>(uid, out var steering) &&
            steering.Status == SteeringStatus.NoPath)
        {
            combatComp.Status = AbilityCombatStatus.TargetUnreachable;
            return;
        }

        // TODO: When I get parallel operators move this as NPC combat shouldn't be handling this.
        _steering.Register(uid, new EntityCoordinates(combatComp.Target, Vector2.Zero), steering);

        if (combatComp.NextAction > curTime)
            return;

        // Get Actions
        if (!TryComp(uid, out ActionsComponent? actionComp))
            return;

        var actions = GetCurentNPCActions(uid, combatComp.NpcActions, actionComp);

        while (actions.Count > 0)
        {
            if (combatComp.UsedActionsLastUpd >= combatComp.ActionsPerUpd)
                break;

            var act = _random.PickAndTake(actions);

            if (act.Value.MinRange >= distance ||
                act.Value.MaxRange <= distance)
                continue;

            if (TryUseAction(uid, act.Key, distance, combatComp, curTime))
                combatComp.UsedActionsLastUpd++;
        }

        if (combatComp.UsedActionsLastUpd >= combatComp.ActionsPerUpd)
        {
            combatComp.UsedActionsLastUpd = 0;
            combatComp.NextAction = curTime + TimeSpan.FromSeconds(combatComp.ActionsTimeReload);
        }
    }

    private bool TryUseAction(EntityUid uid,
                              EntityUid actionUid,
                              float distance,
                              NPCAbilityCombatComponent combatComp,
                              TimeSpan curTime)
    {

        if (!TryComp(uid, out ActionsComponent? actionComp))
            return false;

        if (!TryComp(actionUid, out MetaDataComponent? actionMeta))
            return false;

        if (!_actions.TryGetActionData(actionUid, out var action))
            return false;

        if (!action.Enabled)
            return false;

        // check for action use prevention
        var attemptEv = new ActionAttemptEvent(uid);
        RaiseLocalEvent(actionUid, ref attemptEv);
        if (attemptEv.Cancelled)
            return false;

        if (action.Cooldown.HasValue && action.Cooldown.Value.End > curTime)
            return false;

        if (action is { Charges: < 1, RenewCharges: true })
            _actions.ResetCharges(actionUid);

        BaseActionEvent? performEvent = null;

        if (action.CheckConsciousness && !_actionBlockerSystem.CanConsciouslyPerformAction(uid))
            return false;

        if (!combatComp.Target.IsValid())
        {
            Log.Error($"Attempted to perform an entity-targeted action without a target! Action: {actionMeta.EntityName}");
            return false;
        }

        // Validate request by checking action blockers and the like:
        switch (action)
        {
            case EntityTargetActionComponent entityAction:
                var targetWorldPos = _transform.GetWorldPosition(combatComp.Target);
                _rotateToFaceSystem.TryFaceCoordinates(uid, targetWorldPos);

                if (!_actions.ValidateEntityTarget(uid, combatComp.Target, (actionUid, entityAction)))
                    return false;

                _adminLogger.Add(LogType.Action,
                    $"{ToPrettyString(uid):user} is performing the {actionMeta.EntityName:action} action (provided by {ToPrettyString(action.Container ?? uid):provider}) targeted at {ToPrettyString(combatComp.Target):target}.");

                if (entityAction.Event != null)
                {
                    entityAction.Event.Target = combatComp.Target;
                    Dirty(actionUid, entityAction);
                    performEvent = entityAction.Event;
                }
                break;
            case WorldTargetActionComponent worldAction:
                var entityCoordinatesTarget = Transform(combatComp.Target).Coordinates;

                if (worldAction.Range < distance)
                {
                    var mapTargetPos = entityCoordinatesTarget.ToMapPos(EntityManager, _transform);
                    var mapUserPos = Transform(uid).Coordinates.ToMapPos(EntityManager, _transform);

                    var direction = mapTargetPos - mapUserPos;
                    var coefficient = worldAction.Range / distance;
                    var delta = new Vector2(direction.X * coefficient, direction.Y * coefficient);
                    entityCoordinatesTarget = new EntityCoordinates(entityCoordinatesTarget.EntityId, mapUserPos + delta);
                }

                _rotateToFaceSystem.TryFaceCoordinates(uid, entityCoordinatesTarget.ToMapPos(EntityManager, _transform));

                if (!_actions.ValidateWorldTarget(uid, entityCoordinatesTarget, (actionUid, worldAction)))
                    return false;

                _adminLogger.Add(LogType.Action,
                    $"{ToPrettyString(uid):user} is performing the {actionMeta.EntityName:action} action (provided by {ToPrettyString(action.Container ?? uid):provider}) targeted at {entityCoordinatesTarget:target}.");

                if (worldAction.Event != null)
                {
                    worldAction.Event.Target = entityCoordinatesTarget;
                    Dirty(actionUid, worldAction);
                    performEvent = worldAction.Event;
                }

                break;
            case InstantActionComponent instantAction:
                if (action.CheckCanInteract && !_actionBlockerSystem.CanInteract(uid, null))
                    return false;

                _adminLogger.Add(LogType.Action,
                    $"{ToPrettyString(uid):user} is performing the {actionMeta.EntityName:action} action provided by {ToPrettyString(action.Container ?? uid):provider}.");

                performEvent = instantAction.Event;
                break;
        }

        if (performEvent != null)
            performEvent.Performer = uid;

        // All checks passed. Perform the action!
        _actions.PerformAction(uid, actionComp, actionUid, action, performEvent, curTime);

        return true;
    }

    private List<KeyValuePair<EntityUid, NPCAction>> GetCurentNPCActions(EntityUid uid, List<NPCAction> npcActions, ActionsComponent component)
    {
        var curActionUids = new List<KeyValuePair<EntityUid, NPCAction>>();

        foreach (var act in component.Actions)
        {
            if (TryComp(act, out MetaDataComponent? meta) &&
                npcActions.FindAll(npcAct => npcAct.ActionId == meta.EntityPrototype!.ID).Count != 0)
                curActionUids.Add(new(act, npcActions.Find(npcAct => npcAct.ActionId == meta.EntityPrototype!.ID)));
        }

        return curActionUids;
    }

}
