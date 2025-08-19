namespace Content.Shared.Exodus.Chat.Channels.Dead;

public sealed partial class DeadClientMessage : BaseChatClientMessage
{
    public override ChatChannel Channel => ChatChannel.Dead;
}
