using Robust.Shared.Serialization;

namespace Content.Shared.Exodus.Chat.Channels.LocalSpeech;

[Serializable, NetSerializable]
public sealed partial class LocalSpeechMessage : BaseChatMessage
{
    public override ChatChannel Channel => ChatChannel.LocalSpeech;
    public LocalSpeechType SpeechType;
    public LocId? SpeechVerb;
}
