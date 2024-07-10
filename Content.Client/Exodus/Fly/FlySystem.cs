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

namespace Content.Client.Exodus.Fly;

public sealed class FlySystem : SharedFlySystem
{
    [Dependency] private readonly ISerializationManager _serManager = default!;
    [Dependency] private readonly AnimationPlayerSystem _player = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeNetworkEvent<FlyAnimationMessage>(OnFlyMessage);
    }

    private void OnFlyMessage(FlyAnimationMessage ev)
    {
        var entity = GetEntity(ev.Entity);
        var xform = Transform(entity);

        if (Deleted(entity) || !TryComp<SpriteComponent>(entity, out var entSprite))
            return;

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
        animationDespawn.Lifetime;

    }

}
