// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.Exodus.Chat.Channels.Emote.Animation;

namespace Content.Client.Exodus.Chat.Channels.Emote.Animation;

public sealed partial class EmoteJitterAnimation : EmoteAnimation
{
    [DataField]
    public float Amplitude = 5;

    [DataField]
    public float Frequency = 2;

    // all logic is server-side
}
