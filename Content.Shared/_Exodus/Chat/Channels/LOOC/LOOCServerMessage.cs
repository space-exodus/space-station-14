// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Robust.Shared.Serialization;

namespace Content.Shared.Exodus.Chat.Channels.LOOC;

[Serializable, NetSerializable]
public sealed partial class LOOCServerMessage : BaseChatServerMessage
{
    public override ChatChannel Channel => ChatChannel.LOOC;
    public NetEntity Sender;
    public LOOCType Type;
    public string Content = string.Empty;
    public string? AdminRank;
    public Color? AdminRankColor; // TODO: update for having specified colors for admin ranks
    public Color? ColorOverride;
}
