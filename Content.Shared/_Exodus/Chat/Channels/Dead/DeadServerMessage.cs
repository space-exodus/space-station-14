namespace Content.Shared.Exodus.Chat.Channels.Dead;

public sealed partial class DeadServerMessage : BaseChatServerMessage
{
    public override ChatChannel Channel => ChatChannel.Dead;
    public NetEntity Sender;
    public string Content = string.Empty;
}
