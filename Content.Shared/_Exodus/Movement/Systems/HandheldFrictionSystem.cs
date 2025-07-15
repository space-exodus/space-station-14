using Content.Shared.Exodus.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Hands.Components;
using Content.Shared.Hands;
using Content.Shared.Inventory;
using Robust.Shared.Containers;

namespace Content.Shared.Exodus.Movement;

public sealed class HandheldFrictionSystem : EntitySystem
{
    [Dependency] private readonly MovementSpeedModifierSystem _move = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HandheldFrictionComponent, GotEquippedHandEvent>(OnGotEquipped);
        SubscribeLocalEvent<HandheldFrictionComponent, GotUnequippedHandEvent>(OnGotUnequipped);
        SubscribeLocalEvent<HandheldFrictionComponent, HeldRelayedEvent<RefreshFrictionModifiersEvent>>(OnRefreshFriction);
    }

    private void OnGotEquipped(Entity<HandheldFrictionComponent> ent, ref GotEquippedHandEvent args)
    {
        _move.RefreshFrictionModifiers(args.User);
    }

    private void OnGotUnequipped(Entity<HandheldFrictionComponent> ent, ref GotUnequippedHandEvent args)
    {
        _move.RefreshFrictionModifiers(args.User);
    }

    private void OnRefreshFriction(Entity<HandheldFrictionComponent> ent, ref HeldRelayedEvent<RefreshFrictionModifiersEvent> args)
    {
        args.Args.ModifyFriction(ent.Comp.Friction, ent.Comp.FrictionNoInput);
        args.Args.ModifyAcceleration(ent.Comp.Acceleration);
    }
}
