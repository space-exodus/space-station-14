using Content.Server.Administration.Managers;
using Content.Server.Preferences.Managers;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Content.Shared.Exodus.Chat;
using Content.Shared.Exodus.Chat.Channels.OOC;
using Content.Shared.Ghost;
using Robust.Shared.Configuration;
using Robust.Shared.Player;

namespace Content.Server.Exodus.Chat.Channels.OOC;

// This system doesn't have any deal with entities actually
// in future, chat handlers will be reorganized in other way to respect ECS architecture
public sealed partial class OOCSystem : EntitySystem
{
    [Dependency] private readonly ISharedChatManager _chat = default!;
    [Dependency] private readonly IConfigurationManager _config = default!;
    [Dependency] private readonly IAdminManager _admin = default!;
    [Dependency] private readonly IServerPreferencesManager _preferences = default!;

    public override void Initialize()
    {
        base.Initialize();

        _chat.OnClientMessage += HandleMessage;
    }

    private void HandleMessage(ICommonSession session, BaseChatClientMessage message)
    {
        if (message is not OOCClientMessage ooc)
            return;

        SendOOCMessage(session, ooc.Message, ooc.Type);
    }

    private void SendOOCMessage(ICommonSession sender, string content, OOCType type)
    {
        if (!CanSend(sender, type))
            return;

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

        var netMsg = new OOCServerMessage()
        {
            SenderName = sender.Name,
            Content = content,
            ColorOverride = colorOverride,
            AdminRank = adminRank,
            Type = type,
            Sender = GetNetEntity(sender.AttachedEntity),
        };

        var filter = Filter.Broadcast();

        foreach (var recipient in filter.Recipients)
        {
            _chat.ServerSendMessage(recipient, netMsg);
        }
    }

    private bool CanSend(ICommonSession sender, OOCType type)
    {
        // admins bypass any checks
        if (_admin.IsAdmin(sender))
            return true;

        switch (type)
        {
            // case OOCType.Admin: // kinda, no need
            //     if (_admin.IsAdmin(sender))
            //         return true;
            //     break;
            case OOCType.Dead:
                if (HasComp<GhostComponent>(sender.AttachedEntity))
                    return true;
                break;
            case OOCType.OOC:
                if (_config.GetCVar(CCVars.OocEnabled))
                    return true;
                break;
        }

        return false;
    }
}
