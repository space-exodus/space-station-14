using Robust.Shared.Serialization;

namespace Content.Shared.Exodus.Chat.Channels.OOC;

[Serializable, NetSerializable]
public sealed partial class OOCMessage : BaseChatMessage
{
    public override ChatChannel Channel => ChatChannel.OOC;
    public NetEntity Sender;
}
