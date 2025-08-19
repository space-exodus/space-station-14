// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.Exodus.Chat.Channels.Emote.Animation;
using Robust.Client.GameObjects;
using Robust.Shared.Utility;

namespace Content.Client.Exodus.Chat.Channels.Emote.Animation;

public sealed partial class EmoteAddOverlayAnimation : EmoteAnimation
{
    public const string OVERLAY_SPRITE_LAYER_KEY = "emote_overlay_animation";

    [DataField(required: true)]
    public SpriteSpecifier Sprite;

    [Dependency] private readonly IEntityManager _entity = default!;
    [Dependency] private readonly IEntitySystemManager _system = default!;
    private SpriteSystem? _sprite;

    public override void Start(Entity<EmoteAnimationComponent> mob)
    {
        base.Start(mob);

        _sprite ??= _system.GetEntitySystem<SpriteSystem>();

        if (!_entity.TryGetComponent<SpriteComponent>(mob, out var sprite))
            return;

        var index = _sprite.LayerMapReserve((mob.Owner, sprite), OVERLAY_SPRITE_LAYER_KEY);

        _sprite.LayerSetSprite((mob.Owner, sprite), index, Sprite);
        _sprite.LayerSetVisible((mob.Owner, sprite), index, true);
    }

    public override void End(Entity<EmoteAnimationComponent> mob)
    {
        base.End(mob);

        _sprite ??= _system.GetEntitySystem<SpriteSystem>();

        if (!_entity.TryGetComponent<SpriteComponent>(mob, out var sprite))
            return;

        var index = _sprite.LayerMapGet((mob.Owner, sprite), OVERLAY_SPRITE_LAYER_KEY);
        _sprite.RemoveLayer((mob.Owner, sprite), index);
    }
}
