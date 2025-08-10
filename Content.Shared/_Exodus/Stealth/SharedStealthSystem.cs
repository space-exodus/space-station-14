// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using System.Linq;
using Content.Shared.Examine;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Exodus.Stealth.Components;
using Robust.Shared.GameStates;
using Robust.Shared.Timing;

namespace Content.Shared.Exodus.Stealth;

public abstract class SharedStealthSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StealthComponent, ComponentGetState>(OnStealthGetState);
        SubscribeLocalEvent<StealthComponent, ComponentHandleState>(OnStealthHandleState);
        SubscribeLocalEvent<StealthComponent, MoveEvent>(OnMove);
        SubscribeLocalEvent<StealthComponent, GetVisibilityModifiersEvent>(OnGetVisibilityModifiers);
        SubscribeLocalEvent<StealthComponent, EntityPausedEvent>(OnPaused);
        SubscribeLocalEvent<StealthComponent, EntityUnpausedEvent>(OnUnpaused);
        SubscribeLocalEvent<StealthComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<StealthComponent, ExamineAttemptEvent>(OnExamineAttempt);
        SubscribeLocalEvent<StealthComponent, ExaminedEvent>(OnExamined);
        SubscribeLocalEvent<StealthComponent, MobStateChangedEvent>(OnMobStateChanged);
    }

    private void OnExamineAttempt(EntityUid uid, StealthComponent component, ExamineAttemptEvent args)
    {
        if (component.RequestsStealth.Count == 0)
            return;

        var lastData = GetMinVisibilityData(component);

        if (lastData != null)
        {
            if (GetVisibility(uid, component) > lastData.ExamineThreshold)
                return;

            // Don't block examine for owner or children of the cloaked entity.
            // Containers and the like should already block examining, so not bothering to check for occluding containers.
            var source = args.Examiner;
            do
            {
                if (source == uid)
                    return;
                source = Transform(source).ParentUid;
            }
            while (source.IsValid());

            args.Cancel();
        }
    }

    private void OnExamined(EntityUid uid, StealthComponent component, ExaminedEvent args)
    {
        if (component.RequestsStealth.Count == 0)
            return;

        var lastData = GetMinVisibilityData(component);

        if (lastData != null)
        {
            args.PushMarkup(Loc.GetString(lastData.ExaminedDesc, ("target", uid)));
        }
    }

    private void OnMobStateChanged(EntityUid uid, StealthComponent component, MobStateChangedEvent args)
    {
        if (component.RequestsStealth.Count == 0)
            return;

        var lastData = GetMinVisibilityData(component);

        if (lastData != null)
        {
            if (args.NewMobState == MobState.Dead)
            {
                if (!lastData.EnabledOnDeath)
                {
                    RemComp<StealthComponent>(uid);
                    return;
                }
            }

            Dirty(uid, component);
        }
    }

    private void OnPaused(EntityUid uid, StealthComponent component, ref EntityPausedEvent args)
    {
        if (TerminatingOrDeleted(uid) || component.RequestsStealth.Count == 0)
            return;

        var lastData = GetMinVisibilityData(component);

        if (lastData != null)
        {
            lastData.LastVisibility = GetVisibility(uid, component);
            component.LastUpdated = null;
            Dirty(uid, component);
        }
    }

    private void OnUnpaused(EntityUid uid, StealthComponent component, ref EntityUnpausedEvent args)
    {
        component.LastUpdated = _timing.CurTime;
        Dirty(uid, component);
    }

    protected virtual void OnInit(EntityUid uid, StealthComponent component, ComponentInit args)
    {
        if (component.LastUpdated != null || Paused(uid))
            return;

        component.LastUpdated = _timing.CurTime;

        Dirty(uid, component);
    }

    private void OnStealthGetState(EntityUid uid, StealthComponent component, ref ComponentGetState args)
    {
        if (component.RequestsStealth.Count == 0)
        {
            RemComp<StealthComponent>(uid);
            return;
        }

        args.State = new StealthComponentState(component.RequestsStealth, component.LastUpdated);
    }

    private void OnStealthHandleState(EntityUid uid, StealthComponent component, ref ComponentHandleState args)
    {
        if (args.Current is not StealthComponentState cast)
            return;

        if (cast.RequestsStealth.Count == 0)
        {
            RemComp<StealthComponent>(uid);
            return;
        }

        var lastData = GetMinVisibilityData(component);

        component.RequestsStealth = cast.RequestsStealth;

        var ev = new StealthRequestChangeEvent();
        RaiseLocalEvent(uid, ev);

        component.LastUpdated = cast.LastUpdated;
    }

    private void OnMove(EntityUid uid, StealthComponent component, ref MoveEvent args)
    {
        if (TerminatingOrDeleted(uid) || component.RequestsStealth.Count == 0)
            return;

        var lastData = GetMinVisibilityData(component);

        if (lastData != null)
        {
            if (lastData.PassiveVisibilityRate == null || lastData.MovementVisibilityRate == null)
                return;

            if (_timing.ApplyingState)
                return;

            if (args.NewPosition.EntityId != args.OldPosition.EntityId)
                return;

            var delta = lastData.MovementVisibilityRate * (args.NewPosition.Position - args.OldPosition.Position).Length();

            if (delta.HasValue)
                ModifyVisibility(uid, delta.Value);
        }
    }

    private void OnGetVisibilityModifiers(EntityUid uid, StealthComponent component, GetVisibilityModifiersEvent args)
    {
        if (component.RequestsStealth.Count == 0)
            return;

        var lastData = GetMinVisibilityData(component);

        if (lastData != null)
        {
            if (!lastData.PassiveVisibilityRate.HasValue)
                return;

            var mod = args.SecondsSinceUpdate * lastData.PassiveVisibilityRate;
            if (mod.HasValue)
                args.FlatModifier += mod.Value;
        }
    }

    /// <summary>
    /// /// Modifies the visibility based on the delta provided.
    /// </summary>
    /// <param name="delta">The delta to be used in visibility calculation.</param>
    public void ModifyVisibility(EntityUid uid, float delta, StealthComponent? component = null)
    {
        if (delta == 0 || !Resolve(uid, ref component) || component.RequestsStealth.Count == 0)
            return;

        var lastData = GetMinVisibilityData(component);

        if (lastData != null)
        {
            if (component.LastUpdated != null)
            {
                lastData.LastVisibility = GetVisibility(uid, component);
                component.LastUpdated = _timing.CurTime;
            }

            lastData.LastVisibility = Math.Clamp(lastData.LastVisibility + delta, lastData.MinVisibility, lastData.MaxVisibility);
            Dirty(uid, component);
        }
    }

    /// <summary>
    /// Sets the visibility directly with no modifications
    /// </summary>
    /// <param name="value">The value to set the visibility to. -1 is fully invisible, 1 is fully visible</param>
    public void SetVisibility(EntityUid uid, float value, StealthComponent? component = null)
    {
        if (!Resolve(uid, ref component) || component.RequestsStealth.Count == 0)
            return;

        var lastData = GetMinVisibilityData(component);

        if (lastData != null)
        {
            lastData.LastVisibility = Math.Clamp(value, lastData.MinVisibility, lastData.MaxVisibility);
            if (component.LastUpdated != null)
                component.LastUpdated = _timing.CurTime;

            Dirty(uid, component);
        }
    }

    /// <summary>
    /// Gets the current visibility from the <see cref="StealthComponent"/>
    /// Use this instead of getting LastVisibility from the component directly.
    /// </summary>
    /// <returns>Returns a calculation that accounts for any stealth change that happened since last update, otherwise
    /// returns based on if it can resolve the component. Note that the returned value may be larger than the components
    /// maximum stealth value if it is currently disabled.</returns>
    public float GetVisibility(EntityUid uid, StealthComponent? component = null)
    {
        if (!Resolve(uid, ref component) || TerminatingOrDeleted(uid) || component.RequestsStealth.Count == 0)
            return 1;

        var lastData = GetMinVisibilityData(component);

        if (lastData != null)
        {
            if (component.LastUpdated == null)
                return lastData.LastVisibility;

            var deltaTime = _timing.CurTime - component.LastUpdated.Value;

            var ev = new GetVisibilityModifiersEvent(uid, component, (float)deltaTime.TotalSeconds, 0f);
            RaiseLocalEvent(uid, ev, false);

            return Math.Clamp(lastData.LastVisibility + ev.FlatModifier, lastData.MinVisibility, lastData.MaxVisibility);
        }

        return 1;
    }

    public bool RequestStealth(EntityUid target, EntityUid requester, StealthData data)
    {
        if (!Exists(target) || TerminatingOrDeleted(target))
            return false;

        if (!HasComp<StealthComponent>(target))
        {
            var stealthComp = EnsureComp<StealthComponent>(target);

            if (!stealthComp.RequestsStealth.ContainsKey(GetNetEntity(requester)))
                stealthComp.RequestsStealth.Add(GetNetEntity(requester), data);


            Dirty(target, stealthComp);
        }
        else
        {
            if (!TryComp<StealthComponent>(target, out var stealthComp))
                return false;

            if (!stealthComp.RequestsStealth.ContainsKey(GetNetEntity(requester)))
                stealthComp.RequestsStealth.Add(GetNetEntity(requester), data);

            Dirty(target, stealthComp);
        }

        return true;
    }

    public bool RemoveRequest(EntityUid requester, EntityUid target)
    {
        if (!Exists(target) || TerminatingOrDeleted(target))
            return false;

        if (!TryComp<StealthComponent>(target, out var stealthComp))
            return false;

        if (!stealthComp.RequestsStealth.ContainsKey(GetNetEntity(requester)))
            return false;

        stealthComp.RequestsStealth.Remove(GetNetEntity(requester));

        if (stealthComp.RequestsStealth.Count == 0)
        {
            RemComp<StealthComponent>(target);
            return true;
        }

        Dirty(target, stealthComp);

        return true;
    }

    private StealthData? GetMinVisibilityData(StealthComponent component)
    {
        StealthData? returnData = null;
        float minValue = float.MaxValue;

        if (component.RequestsStealth.Count == 0)
            return null;

        foreach (var data in component.RequestsStealth.Values)
        {
            var clamped = Math.Clamp(data.LastVisibility, data.MinVisibility, data.MaxVisibility);

            if (clamped < minValue)
            {
                minValue = clamped;
                returnData = data;
            }
        }

        return returnData;
    }

    /// <summary>
    ///     Used to run through any stealth effecting components on the entity.
    /// </summary>
    private sealed class GetVisibilityModifiersEvent : EntityEventArgs
    {
        public readonly StealthComponent Stealth;
        public readonly float SecondsSinceUpdate;

        /// <summary>
        ///     Calculate this and add to it. Do not divide, multiply, or overwrite.
        ///     The sum will be added to the stealth component's visibility.
        /// </summary>
        public float FlatModifier;

        public GetVisibilityModifiersEvent(EntityUid uid, StealthComponent stealth, float secondsSinceUpdate, float flatModifier)
        {
            Stealth = stealth;
            SecondsSinceUpdate = secondsSinceUpdate;
            FlatModifier = flatModifier;
        }
    }
}


public sealed class StealthRequestChangeEvent : EntityEventArgs
{
    public StealthRequestChangeEvent() { }
}
