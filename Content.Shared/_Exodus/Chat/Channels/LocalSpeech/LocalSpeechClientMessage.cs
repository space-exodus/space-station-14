using Robust.Shared.Serialization;

namespace Content.Shared.Exodus.Chat.Channels.LocalSpeech;

[Serializable, NetSerializable]
public sealed partial class LocalSpeechClientMessage : BaseChatClientMessage
{
    public override ChatChannel Channel => ChatChannel.LocalSpeech;
}
