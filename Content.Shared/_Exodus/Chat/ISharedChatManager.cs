using Robust.Shared.Player;

namespace Content.Shared.Exodus.Chat;

public interface ISharedChatManager
{
    public event Action<BaseChatServerMessage>? OnServerMessage;
    public event Action<ICommonSession, BaseChatClientMessage>? OnClientMessage;

    public void Initialize();

    public void ServerSendMessage(EntityUid entity, BaseChatServerMessage message);

    public void ServerSendMessage(ICommonSession session, BaseChatServerMessage message);

    public void ClientSendMessage(BaseChatClientMessage message);
}