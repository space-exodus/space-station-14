using Content.Shared.Actions;
using Content.Shared.Actions.Events;

namespace Content.Shared.Exodus.Abilities;
public abstract partial class SharedAbilitiesSystem
{
    private void InitializeDoAbility()
    {
        SubscribeLocalEvent<WorldTargetActionComponent, ActionAttemptEvent>(OnActionAttemt);
        SubscribeLocalEvent<InstantActionComponent, ActionAttemptEvent>(OnActionAttemt);
        SubscribeLocalEvent<EntityTargetActionComponent, ActionAttemptEvent>(OnActionAttemt);
    }

    private void UpdateDoAbility(float frameTime)
    {
        Dictionary<PerformingAbilityGroup, EntityUid> riseGroups = [];

        var query = EntityQueryEnumerator<DoAbilityComponent>();
        while (query.MoveNext(out var performer, out var doAbilityComp))
        {
            List<PerformingAbilityGroup> delGroups = [];
            foreach (var group in doAbilityComp.Groups)
            {
                if (Timing.CurTime >= group.TimeEnd)
                {
                    delGroups.Add(group);
                    riseGroups.Add(group, performer);
                }
            }
            doAbilityComp.Groups.RemoveAll(delGroups.Contains);
            if (doAbilityComp.Groups.Count == 0)
                RemCompDeferred(performer, doAbilityComp);
        }

        foreach (var (group, performer) in riseGroups)
        {
            if (group.ExecutableEvent is BaseActionEvent ev &&
                group.Target is BaseActionEvent targ)
                RaiseAbilityEvent(performer, ev, targ);
        }
    }

    public void AddOccuringAbility(EntityUid performer, float duration, BaseActionEvent? ev = null, BaseActionEvent? target = null)
    {
        var doAbilityComp = EnsureComp<DoAbilityComponent>(performer);

        if (ev is BaseActionEvent evToRise && target is BaseActionEvent targ)
        {
            doAbilityComp.Groups.Add(new PerformingAbilityGroup()
            {
                TimeEnd = Timing.CurTime + TimeSpan.FromSeconds(duration),
                ExecutableEvent = evToRise,
                Target = targ
            });
        }
        else
        {
            doAbilityComp.Groups.Add(new PerformingAbilityGroup() { TimeEnd = Timing.CurTime + TimeSpan.FromSeconds(duration) });
        }


    }

    private void OnActionAttemt(EntityUid actionUid, BaseActionComponent component, ActionAttemptEvent ev)
    {
        if (TryComp<DoAbilityComponent>(ev.User, out var doAbilityComp))
            ev.Cancelled = doAbilityComp.SuppressActions;
    }
}
