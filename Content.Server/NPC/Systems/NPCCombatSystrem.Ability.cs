// Exodus-AdvancedAI
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
using Content.Server.Charges;
using Content.Shared.Actions.Components;

namespace Content.Server.NPC.Systems;

public sealed partial class NPCCombatSystem
{
    [Dependency] private readonly ActionsSystem _actions = default!;
    [Dependency] private readonly ActionBlockerSystem _actionBlockerSystem = default!;
    [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;
    [Dependency] private readonly RotateToFaceSystem _rotateToFaceSystem = default!;
    [Dependency] private readonly ChargesSystem _chargesSystem = default!;

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
            CastActions(uid, comp, curTime, physicsQuery, xformQuery);
        }
    }

    private void CastActions(EntityUid uid, NPCAbilityCombatComponent combatComp, TimeSpan curTime, EntityQuery<PhysicsComponent> physicsQuery, EntityQuery<TransformComponent> xformQuery)
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

        _steering.Register(uid, new EntityCoordinates(combatComp.Target, Vector2.Zero), steering);

        if (combatComp.NextAction > curTime)
            return;

        // Get Actions
        if (!TryComp(uid, out ActionsComponent? actionComp))
            return;

        List<EntityUid> actions = [];
        actions.AddRange(actionComp.Actions);

        while (actions.Count > 0)
        {
            if (combatComp.UsedActionsLastUpd >= combatComp.ActionsPerUpd)
                break;

            var act = _random.PickAndTake(actions);

            var attemptEv = new ActionAttemptEvent(uid);
            RaiseLocalEvent(act, ref attemptEv);
            if (attemptEv.Cancelled)
                return;

            if (TryUseAction(uid, act, distance, combatComp, curTime))
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
        if (!HasComp<ActionsComponent>(uid))
            return false;

        var action = _actions.GetAction(actionUid);

        if (action == null)
        {
            Log.Error($"Tried to perform an invalid action with uid: {actionUid}");
            return false;
        }

        var actionComp = action.Value.Comp;

        if (!actionComp.Enabled || !actionComp.UsableByNPC)
            return false;

        // check for action use prevention
        var attemptEv = new ActionAttemptEvent(uid);
        RaiseLocalEvent(actionUid, ref attemptEv);

        if (attemptEv.Cancelled)
            return false;

        if (actionComp.Cooldown.HasValue && actionComp.Cooldown.Value.End > curTime)
            return false;

        // if action is rechargable and needs recharge we'll reset charges
        if (_chargesSystem.GetNextRechargeTime(actionUid) <= TimeSpan.Zero)
            _chargesSystem.ResetCharges(actionUid);

        BaseActionEvent? performEvent = _actions.GetEvent(actionUid);

        if (performEvent == null)
        {
            Log.Error("Tried to perform an invalid action which doesn't have any event");
            return false;
        }

        // Perform all needed validation for action like if it was used just by player
        // TODO: rewrite it when proper way for manual calling of action will be available
        var provider = actionComp.Container ?? uid;
        var validateActionEv = new ActionValidateEvent()
        {
            Input = new RequestPerformActionEvent(GetNetEntity(actionUid), GetNetEntity(combatComp.Target)),
            Provider = provider,
            User = uid,
        };
        RaiseLocalEvent(actionUid, ref validateActionEv);

        if (validateActionEv.Invalid)
            return false;

        if (actionComp.MinAIUseRange >= distance ||
            actionComp.MaxAIUseRange <= distance)
        {
            return false;
        }

        // All checks passed. Perform the action!
        _actions.PerformAction(uid, (actionUid, actionComp), performEvent);

        return true;
    }

}
