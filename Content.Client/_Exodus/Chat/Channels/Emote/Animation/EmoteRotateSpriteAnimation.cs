// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Client.Rotation;
using Content.Shared.Exodus.Chat.Channels.Emote.Animation;
using Robust.Client.GameObjects;

namespace Content.Client.Exodus.Chat.Channels.Emote.Animation;

public sealed partial class EmoteRotateSpriteAnimation : EmoteAnimation
{
    [DataField(required: true)]
    public Angle Rotation;

    [Dependency] private readonly IEntitySystemManager _system = default!;
    [Dependency] private readonly IEntityManager _entity = default!;

    private RotationVisualizerSystem? _rotation;

    public override void Start(Entity<EmoteAnimationComponent> mob)
    {
        base.Start(mob);

        _rotation ??= _system.GetEntitySystem<RotationVisualizerSystem>();

        if (!_entity.TryGetComponent<SpriteComponent>(mob, out var sprite))
            return;

        _rotation.AnimateSpriteRotation(mob, sprite, Rotation, Playtime);
    }

    public override void End(Entity<EmoteAnimationComponent> mob)
    {
        base.End(mob);

        _rotation ??= _system.GetEntitySystem<RotationVisualizerSystem>();

        if (!_entity.TryGetComponent<SpriteComponent>(mob, out var sprite))
            return;

        _rotation.StopRotationAnimation(mob, sprite);
    }
}
