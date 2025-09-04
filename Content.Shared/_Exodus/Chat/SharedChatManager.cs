using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Shared.Exodus.Chat;

public sealed partial class SharedChatManager : ISharedChatManager
{
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly ISharedPlayerManager _player = default!;

    public event Action<BaseChatServerMessage>? OnServerMessage;
    public event Action<ICommonSession, BaseChatClientMessage>? OnClientMessage;

    public void Initialize()
    {
        _net.RegisterNetMessage<ChatServerNetMessage>(HandleServerMessage, accept: NetMessageAccept.Client);
        _net.RegisterNetMessage<ChatClientNetMessage>(HandleClientMessage, accept: NetMessageAccept.Server);
    }

    private void HandleClientMessage(ChatClientNetMessage message)
    {
        if (message.Message is not { } content)
            return;

        // TODO: rate-limit check
        // TODO: global channels blocking
        // TODO: individual channels blocking by admins

        OnClientMessage?.Invoke(_player.GetSessionByChannel(message.MsgChannel), content);
    }

    private void HandleServerMessage(ChatServerNetMessage message)
    {
        if (message.Message is not { } content)
            return;

        OnServerMessage?.Invoke(content);
    }

    public void ServerSendMessage(EntityUid entity, BaseChatServerMessage message)
    {
        if (!_player.TryGetSessionByEntity(entity, out var session))
            return;

        ServerSendMessage(session, message);
    }

    public void ServerSendMessage(ICommonSession session, BaseChatServerMessage message)
    {
        var netMsg = new ChatServerNetMessage()
        {
            Message = message,
        };
        _net.ServerSendMessage(netMsg, session.Channel);
    }

    public void ClientSendMessage(BaseChatClientMessage message)
    {
        var netMsg = new ChatClientNetMessage()
        {
            Message = message,
        };

        _net.ClientSendMessage(netMsg);
    }
}