// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.Exodus.Chat.Channels;
using Robust.Shared.Serialization;

namespace Content.Shared.Exodus.Chat;

[Serializable, NetSerializable]
public abstract class BaseChatServerMessage
{
    public abstract ChatChannel Channel { get; }
    public string? SenderName { get; set; }
    /// <summary>
    /// Whether or not this message should be shown in chatbox
    /// </summary>
    public bool HideChat { get; set; }
}
