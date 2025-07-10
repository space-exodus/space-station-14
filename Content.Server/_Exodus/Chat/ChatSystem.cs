using Content.Shared.Exodus.Chat;
using Robust.Shared.Player;

namespace Content.Server.Exodus.Chat;

public sealed partial class ChatSystem : SharedChatSystem
{
    public event Action<ICommonSession, BaseChatClientMessage>? OnHandleMessage;

    public void HandleClientMessage(ICommonSession player, BaseChatClientMessage message)
    {
        // TODO: rate-limit check
        // TODO: global channels blocking
        // TODO: individual channels blocking by admins

        OnHandleMessage?.Invoke(player, message);
    }
}
