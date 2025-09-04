using Content.Server.Administration.Managers;
using Content.Server.Preferences.Managers;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Content.Shared.Exodus.Chat;
using Content.Shared.Exodus.Chat.Channels.LOOC;
using Content.Shared.Exodus.Utils;
using Content.Shared.Roles;
using Robust.Shared.Configuration;
using Robust.Shared.Player;

namespace Content.Server.Exodus.Chat.Channels.LOOC;

// This system doesn't have any deal with entities actually
// in future, chat handlers will be reorganized in other way to respect ECS architecture
public sealed partial class LOOCSystem : EntitySystem
{
    [Dependency] private readonly ISharedChatManager _chat = default!;
    [Dependency] private readonly IConfigurationManager _config = default!;
    [Dependency] private readonly IAdminManager _admin = default!;
    [Dependency] private readonly IServerPreferencesManager _preferences = default!;
    [Dependency] private readonly SharedRoleSystem _role = default!;

    public override void Initialize()
    {
        base.Initialize();

        _chat.OnClientMessage += HandleMessage;
    }

    private void HandleMessage(ICommonSession session, BaseChatClientMessage message)
    {
        if (message is not LOOCClientMessage looc)
            return;

        if (session.AttachedEntity is not { } senderUid)
            return;

        if (!CanSend(session, looc.Type))
            return;

        SendLOOCMessage(session, GetNetEntity(senderUid), looc.Message, looc.Type);
    }

    private void SendLOOCMessage(ICommonSession sender, NetEntity senderUid, string content, LOOCType type)
    {
        Color? colorOverride = null;
        string? adminRank = null;

        if (_admin.IsAdmin(sender) && _admin.HasAdminFlag(sender, AdminFlags.NameColor))
        {
            var adminData = _admin.GetAdminData(sender);
            if (adminData != null)
                adminRank = adminData.Title;

            var prefs = _preferences.GetPreferencesOrNull(sender.UserId);
            if (prefs != null)
                colorOverride = prefs.AdminOOCColor;
        }

        var netMsg = new LOOCServerMessage()
        {
            SenderName = sender.Name,
            Content = content,
            ColorOverride = colorOverride,
            AdminRank = adminRank,
            Type = type,
            Sender = senderUid,
        };

        var filter = Filter.Pvs(GetEntity(senderUid));

        foreach (var recipient in filter.Recipients)
        {
            _chat.ServerSendMessage(recipient, netMsg);
        }
    }

    private bool CanSend(ICommonSession sender, LOOCType type)
    {
        // admins bypass any checks
        if (_admin.IsAdmin(sender))
            return true;

        if (!_config.GetCVar(CCVars.LoocEnabled))
            return false;

        if (!this.IsInConscious(sender.AttachedEntity, EntityManager))
            return false;

        if (type == LOOCType.AntagLOOC && !_role.MindIsAntagonist(sender.AttachedEntity))
            return false;

        return false;
    }
}
