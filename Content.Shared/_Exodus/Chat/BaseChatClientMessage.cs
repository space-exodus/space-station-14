using Content.Shared.Exodus.Chat.Channels;
using Robust.Shared.Serialization;

namespace Content.Shared.Exodus.Chat;

[Serializable, NetSerializable]
public abstract class BaseChatClientMessage
{
    public string Message { get; } = string.Empty;
    public abstract ChatChannel Channel { get; }
}
