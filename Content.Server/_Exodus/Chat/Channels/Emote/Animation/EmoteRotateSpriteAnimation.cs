// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.Exodus.Chat.Channels.Emote.Animation;

namespace Content.Server.Exodus.Chat.Channels.Emote.Animation;

public sealed partial class EmoteRotateSpriteAnimation : EmoteAnimation
{
    [DataField(required: true)]
    public Angle Rotation;

    // all logic is client-side
}
