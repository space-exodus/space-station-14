using Content.Server.Actions;
using Content.Shared.Actions;
using Content.Shared.Exodus.Bioluminescence;
using Content.Shared.Humanoid;

namespace Content.Server.Exodus.Lavaland;
public sealed class AbilityAttackSystem : EntitySystem
{
    [Dependency] private readonly ActionsSystem _actions = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<AbilityAttackComponent, ComponentStartup>(OnStartUp);
    }

    private void OnStartUp(EntityUid uid, AbilityAttackComponent component, ComponentStartup args)
    {
        if (!TryComp<ActionsComponent>(uid, out var actionComp))
            return;

        foreach (var act in component.Actions)
        {
            _actions.AddAction(uid, act, uid, actionComp);
        }
    }
}
