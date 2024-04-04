using System.Linq;
using System.Threading.Tasks;
using Content.Server.Database;
using Content.Shared.Roles;
using Robust.Shared.Network;
using Robust.Shared.Utility;

namespace Content.Server.Roles;

public sealed class RoleWhitelistManager
{
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly IServerDbManager _db = default!;

    private readonly Dictionary<NetUserId, RoleWhitelistInfo> _cachedWhitelist = [];

    public void Initialize()
    {
        _net.RegisterNetMessage<MsgRoleWhitelistInfo>();

        _net.Connecting += OnConnecting;
        _net.Connected += OnConnected;
        _net.Disconnect += OnDisconnect;
    }

    private async Task OnConnecting(NetConnectingArgs args)
    {
        var user = args.UserData;

        DebugTools.Assert(!_cachedWhitelist.ContainsKey(user.UserId), "Cached data was found on client connect");

        var whitelistInfo = await GetRoleWhitelistInfo(user.UserId);

        _cachedWhitelist.Add(user.UserId, whitelistInfo);
    }

    private void OnConnected(object? sender, NetChannelArgs args)
    {
        var info = _cachedWhitelist.TryGetValue(args.Channel.UserId, out var whitelist) ? whitelist : null;
        var msg = new MsgRoleWhitelistInfo() { Info = info };
        _net.ServerSendMessage(msg, args.Channel);
    }

    private void OnDisconnect(object? sender, NetDisconnectedArgs args)
    {
        _cachedWhitelist.Remove(args.Channel.UserId);
    }

    public async Task<RoleWhitelistInfo> GetRoleWhitelistInfo(NetUserId userId)
    {
        var rolesWhitelist = await _db.GetRoleWhitelist(userId);
        var rolesGroupsWhitelist = await _db.GetRolesGroupWhitelist(userId);

        var info = new RoleWhitelistInfo()
        {
            Roles = rolesWhitelist,
            RolesGroups = rolesGroupsWhitelist,
        };

        return info;
    }
}
