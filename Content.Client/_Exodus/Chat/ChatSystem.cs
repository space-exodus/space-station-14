// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.Exodus.Chat;
using Robust.Shared.Network;

namespace Content.Client.Exodus.Chat;

public sealed partial class ChatSystem : SharedChatSystem
{
    [Dependency] private readonly INetManager _net = default!;

    public event Action<BaseChatServerMessage>? OnHandleMessage;

    public override void Initialize()
    {
        base.Initialize();

        _net.RegisterNetMessage<ChatServerNetMessage>(HandleServerMessage, accept: NetMessageAccept.Client);
        _net.RegisterNetMessage<ChatClientNetMessage>(accept: NetMessageAccept.Server);
    }

    private void HandleServerMessage(ChatServerNetMessage message)
    {
        if (message.Message is not { } content)
            return;

        OnHandleMessage?.Invoke(content);
    }

    public void SendNetworkMessage(BaseChatClientMessage message)
    {
        var netMsg = new ChatClientNetMessage()
        {
            Message = message,
        };

        _net.ClientSendMessage(netMsg);
    }
}
