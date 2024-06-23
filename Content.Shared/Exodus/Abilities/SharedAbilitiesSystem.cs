using Content.Shared.Actions;
using Content.Shared.Actions.Events;
using Content.Shared.Exodus.Abilities.Events;
using Robust.Shared.Timing;

namespace Content.Shared.Exodus.Abilities;

public abstract partial class SharedAbilitiesSystem : EntitySystem
{
    [Dependency] protected readonly SharedTransformSystem _transform = default!;
    [Dependency] protected readonly SharedActionsSystem _actions = default!;
    [Dependency] protected readonly IGameTiming _timing = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<AbilitiesComponent, ComponentStartup>(OnStartUp);

        InitializeAbilityCast();
        InitializeDoAbility();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        UpdateDoAbility(frameTime);
    }

    private void OnStartUp(EntityUid uid, AbilitiesComponent component, ComponentStartup args)
    {
        if (!TryComp<ActionsComponent>(uid, out var actionComp))
            return;

        foreach (var act in component.Actions)
        {
            _actions.AddAction(uid, act, uid, actionComp);
        }
    }
}
