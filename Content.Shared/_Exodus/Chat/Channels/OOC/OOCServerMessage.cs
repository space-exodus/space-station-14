// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Robust.Shared.Serialization;

namespace Content.Shared.Exodus.Chat.Channels.OOC;

[Serializable, NetSerializable]
public sealed partial class OOCServerMessage : BaseChatServerMessage
{
    public override ChatChannel Channel => ChatChannel.OOC;
    public NetEntity Sender;
    public string Content = string.Empty;
}
