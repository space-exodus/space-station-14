// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

namespace Content.Shared.Exodus.Chat.Channels.Emote;

[ByRefEvent]
public sealed partial class EmoteEvent : EntityEventArgs
{
    public EntityUid Emoting;
    public EmotePrototype? Emote;
    public string EmoteMessage = string.Empty;
}
