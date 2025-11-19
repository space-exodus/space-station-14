// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.Exodus.Chat.Channels.Emote.Animation;
using Robust.Shared.Utility;

namespace Content.Server.Exodus.Chat.Channels.Emote.Animation;

public sealed partial class EmoteAddOverlayAnimation : EmoteAnimation
{
    [DataField(required: true)]
    public SpriteSpecifier Sprite;

    // all logic is client side
}
