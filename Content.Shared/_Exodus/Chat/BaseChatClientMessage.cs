// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.Exodus.Chat.Channels;
using Robust.Shared.Serialization;

namespace Content.Shared.Exodus.Chat;

[Serializable, NetSerializable]
public abstract class BaseChatClientMessage
{
    public string Message { get; } = string.Empty;
    public abstract ChatChannel Channel { get; }
}
