using System.Numerics;
using Content.Shared.Salvage.Fulton;
using Robust.Shared.Spawners;
using JetBrains.Annotations;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Animations;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Utility;
using TimedDespawnComponent = Robust.Shared.Spawners.TimedDespawnComponent;
using Content.Shared.Exodus.Fly;
using TerraFX.Interop.Xlib;

using Content.Client.Movement.Systems;
using Content.Shared.Actions;
using Content.Shared.Ghost;
using Robust.Client.Console;
using Robust.Client.Player;
using Robust.Shared.Player;

namespace Content.Client.Exodus.Fly;

public sealed class FlySystem : SharedFlySystem
{
    [Dependency] private readonly ISerializationManager _serManager = default!;
    [Dependency] private readonly AnimationPlayerSystem _player = default!;
    [Dependency] private readonly ContentEyeSystem _contentEye = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeNetworkEvent<TakeoffAnimationMessage>(OnTakeoffAnimationMessage);
        SubscribeNetworkEvent<LandAnimationMessage>(OnLandAnimationMessage);

        SubscribeNetworkEvent<TakeoffMessage>(OnTakeoffMessage);
        SubscribeNetworkEvent<LandMessage>(OnLandMessage);
    }


    private void OnTakeoffAnimationMessage(TakeoffAnimationMessage ev)
    {
        var entity = GetEntity(ev.Entity);

        if (Deleted(entity) ||
            !TryComp<SpriteComponent>(entity, out var entSprite) ||
            !TryComp<FlyComponent>(entity, out var flyComp))
            return;


        var effectEnt = SpawnEffect(entity, flyComp.TakeoffTime);

        if (effectEnt == EntityUid.Invalid)
            return;

        var flyLayer = entSprite.LayerMapGet(Flying.InAir);

        _player.Play(); // TODO
    }

    private void OnLandAnimationMessage(LandAnimationMessage ev)
    {
        var entity = GetEntity(ev.Entity);

        if (!TryComp<FlyComponent>(entity, out var flyComp))
            return;
        var effectEnt = SpawnEffect(entity, flyComp.LandTime);
        if (effectEnt == EntityUid.Invalid)
            return;

        if (TryComp<EyeComponent>(entity, out var eyeComp) && eyeComp.DrawFov)
            _contentEye.RequestToggleFov();

        _player.Play(); // TODO
    }

    private void OnTakeoffMessage(TakeoffMessage ev)
    {
        var entity = GetEntity(ev.Entity);

        if (TryComp<EyeComponent>(entity, out var eyeComp) && eyeComp.DrawFov)
            _contentEye.RequestToggleFov();

    }

    private void OnLandMessage(LandMessage ev)
    {
        var entity = GetEntity(ev.Entity);

        if (Deleted(entity) ||
            !TryComp<SpriteComponent>(entity, out var entSprite) ||
            !TryComp<FlyComponent>(entity, out var flyComp))
            return;

        entSprite.Visible = true;
    }

    private EntityUid SpawnEffect(EntityUid entity, float lifetime)
    {
        var xform = Transform(entity);

        if (Deleted(entity) ||
            !TryComp<SpriteComponent>(entity, out var entSprite) ||
            !TryComp<FlyComponent>(entity, out var flyComp))
            return EntityUid.Invalid;

        var animationEnt = Spawn(null, xform.Coordinates);
        var animationSprite = AddComp<SpriteComponent>(animationEnt);

        _serManager.CopyTo(entSprite, ref animationSprite, notNullableOverride: true);

        if (TryComp<AppearanceComponent>(entity, out var entAppearance))
        {
            var animationAppearance = AddComp<AppearanceComponent>(animationEnt);
            _serManager.CopyTo(entAppearance, ref animationAppearance, notNullableOverride: true);
        }

        animationSprite.NoRotation = true;

        var animationDespawn = AddComp<TimedDespawnComponent>(animationEnt);
        animationDespawn.Lifetime = lifetime;

        return animationEnt;
    }

    private enum Flying
    {
        InAir,
    }

}
