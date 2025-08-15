// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.Exodus.Chat;
using Robust.Server.Player;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Server.Exodus.Chat;

public sealed partial class ChatSystem : SharedChatSystem
{
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly IPlayerManager _player = default!;

    public event Action<ICommonSession, BaseChatClientMessage>? OnHandleMessage;

    public override void Initialize()
    {
        base.Initialize();

        _net.RegisterNetMessage<ChatServerNetMessage>(accept: NetMessageAccept.Client);
        _net.RegisterNetMessage<ChatClientNetMessage>(HandleClientMessage, accept: NetMessageAccept.Server);
    }

    public void HandleClientMessage(ChatClientNetMessage message)
    {
        if (message.Message is not { } content)
            return;

        // TODO: rate-limit check
        // TODO: global channels blocking
        // TODO: individual channels blocking by admins

        OnHandleMessage?.Invoke(_player.GetSessionByChannel(message.MsgChannel), content);
    }

    public void SendNetworkMessage(Entity<ActorComponent?> entity, BaseChatServerMessage message)
    {
        if (!Resolve(entity, ref entity.Comp, false))
            return;

        var actor = entity.Comp;

        var netMsg = new ChatServerNetMessage()
        {
            Message = message,
        };
        _net.ServerSendMessage(netMsg, actor.PlayerSession.Channel);
    }

    public string GetSenderNameInitial(EntityUid sender)
    {
        return MetaData(sender).EntityName;
    }

    public string GetSenderNameForRecipient(EntityUid sender, string initialSenderName, EntityUid recipient)
    {
        return initialSenderName;
    }
}
