using Content.Server.Administration.Managers;
using Content.Shared.Exodus.Chat;
using Content.Shared.Exodus.Chat.Channels.Dead;
using Robust.Shared.Player;

namespace Content.Server.Exodus.Chat.Channels.Dead;

public sealed partial class DeadChatSystem : EntitySystem
{
    [Dependency] private readonly ISharedChatManager _chat = default!;
    [Dependency] private readonly IAdminManager _admin = default!;
    [Dependency] private readonly ChatIdentitySystem _chatIdentity = default!;

    public override void Initialize()
    {
        base.Initialize();

        _chat.OnClientMessage += HandleMessage;
    }

    private void HandleMessage(ICommonSession session, BaseChatClientMessage message)
    {
        if (session.AttachedEntity is not { } sender)
            return;

        if (message is not DeadClientMessage dead)
            return;

        var filter = Filter.Pvs(sender, 0.5f);

        foreach (var recipient in filter.Recipients)
        {
            if (recipient.AttachedEntity is not { } recipientUid)
                continue;

            if (!HasComp<GhostHearingComponent>(recipientUid) && !_admin.IsAdmin(recipient))
                continue;

            SendMessage(sender, dead.Message, recipientUid);
        }
    }

    private void SendMessage(EntityUid sender, string content, EntityUid recipient)
    {
        var message = new DeadServerMessage()
        {
            Sender = GetNetEntity(sender),
            SenderName = _chatIdentity.GetSenderNameInitial(sender),
            Content = content,
        };

        _chat.ServerSendMessage(recipient, message);
    }
}
