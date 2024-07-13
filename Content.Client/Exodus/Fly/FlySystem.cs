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


namespace Content.Client.Exodus.Fly;

public sealed class FlySystem : SharedFlySystem
{
    [Dependency] private readonly ISerializationManager _serManager = default!;
    [Dependency] private readonly AnimationPlayerSystem _player = default!;
    [Dependency] private readonly ContentEyeSystem _contentEye = default!;
    [Dependency] private readonly TransformSystem _transform = default!;


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

        // Create effect ent that would be shown on flying ent coordinates
        var groundEffect = SpawnGroundEffect(entity);
        flyComp.Effect = groundEffect;
        _transform.SetParent(groundEffect, entity);

        // Create takeoff animation ent
        var effectEnt = SpawnFlyEffect(entity, flyComp.TakeoffTime);
        if (effectEnt == EntityUid.Invalid)
            return;
        var takeoffAnim = new Animation()
        {
            Length = TimeSpan.FromSeconds(flyComp.TakeoffTime),
            AnimationTracks =
            {
                new AnimationTrackComponentProperty()
                {
                    ComponentType = typeof(SpriteComponent),
                    Property = nameof(SpriteComponent.Offset),
                    KeyFrames =
                    {
                        new AnimationTrackProperty.KeyFrame(Vector2.Zero, 0f),
                        new AnimationTrackProperty.KeyFrame(new Vector2((float) Math.Sin(flyComp.TakeoffAngle) * 20f,
                                                                        (float) Math.Cos(flyComp.TakeoffAngle) * 20f), flyComp.TakeoffTime)
                    }
                }
            }
        };
        _player.Play(effectEnt, takeoffAnim, "takeoff-animation");

        entSprite.Visible = false;
    }

    private void OnLandAnimationMessage(LandAnimationMessage ev)
    {
        var entity = GetEntity(ev.Entity);

        if (!TryComp<FlyComponent>(entity, out var flyComp))
            return;

        // Create landig animation ent
        var effectEnt = SpawnFlyEffect(entity, flyComp.LandTime);
        if (effectEnt == EntityUid.Invalid)
            return;
        _transform.SetParent(effectEnt, entity);
        var landAnim = new Animation()
        {
            Length = TimeSpan.FromSeconds(flyComp.LandTime),
            AnimationTracks =
            {
                new AnimationTrackComponentProperty()
                {
                    ComponentType = typeof(SpriteComponent),
                    Property = nameof(SpriteComponent.Offset),
                    KeyFrames =
                    {
                        new AnimationTrackProperty.KeyFrame(new Vector2((float) Math.Sin(flyComp.LandAngle) * 20f,
                                                                        (float) Math.Cos(flyComp.LandTime) * 20f), 0f),
                        new AnimationTrackProperty.KeyFrame(Vector2.Zero, flyComp.LandTime)
                    }
                }
            }
        };
        _player.Play(effectEnt, landAnim, "land-animation");

        // Turn off FOV
        if (TryComp<EyeComponent>(entity, out var eyeComp) && eyeComp.DrawFov)
            _contentEye.RequestToggleFov();
    }

    private void OnTakeoffMessage(TakeoffMessage ev)
    {
        var entity = GetEntity(ev.Entity);

        // Turn on FOV
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

        // Delete ground effect
        if (!Deleted(flyComp.Effect))
            QueueDel(flyComp.Effect);

        entSprite.Visible = true;
    }

    private EntityUid SpawnFlyEffect(EntityUid entity, float lifetime)
    {
        var xform = Transform(entity);

        if (Deleted(entity) ||
            !TryComp<SpriteComponent>(entity, out var entSprite) ||
            !TryComp<FlyComponent>(entity, out var flyComp))
            return EntityUid.Invalid;

        var animationEnt = Spawn(null, xform.Coordinates);
        var animationSprite = AddComp<SpriteComponent>(animationEnt);

        _serManager.CopyTo(entSprite, ref animationSprite, notNullableOverride: true);
        animationSprite.Visible = true;

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

    private EntityUid SpawnGroundEffect(EntityUid entity)
    {
        var xform = Transform(entity);

        if (Deleted(entity) ||
            !TryComp<FlyComponent>(entity, out var flyComp))
            return EntityUid.Invalid;

        var animationEnt = Spawn(null, xform.Coordinates);
        var animationSprite = AddComp<SpriteComponent>(animationEnt);

        if (flyComp.GroundEffectRsi != null)
            animationSprite.AddLayer(new SpriteSpecifier.Rsi(flyComp.GroundEffectRsi.Value, "in-air"));

        return animationEnt;
    }
}
