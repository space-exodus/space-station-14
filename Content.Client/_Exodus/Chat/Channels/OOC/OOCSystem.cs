using Content.Client.Administration.Managers;
using Content.Shared.CCVar;
using Content.Shared.Exodus.Chat;
using Content.Shared.Exodus.Chat.Channels.OOC;
using Robust.Shared.Configuration;
using Robust.Shared.Player;

namespace Content.Client.Exodus.Chat.Channels.OOC;

// This system doesn't have any deal with entities actually
// in future, chat handlers will be reorganized in other way to respect ECS architecture
public sealed partial class OOCSystem : EntitySystem
{
    [Dependency] private readonly ISharedChatManager _chat = default!;
    [Dependency] private readonly IConfigurationManager _config = default!;
    [Dependency] private readonly IClientAdminManager _admin = default!;

    public override void Initialize()
    {
        base.Initialize();

        _chat.OnServerMessage += HandleMessage;
    }

    private void HandleMessage(BaseChatServerMessage message)
    {
        if (message is not OOCServerMessage ooc)
            return;

        SendOOCMessage(ooc);
    }

    private void SendOOCMessage(OOCServerMessage message)
    {
        // add message to ChatBox
    }

    public bool CanSend(ICommonSession sender)
    {
        if (_admin.IsAdmin() ||
            _config.GetCVar(CCVars.OocEnabled))
            return true;

        return false;
    }
}
