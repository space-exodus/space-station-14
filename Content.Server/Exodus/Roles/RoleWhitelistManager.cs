using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Content.Server.Database;
using Content.Shared.Roles;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server.Roles;

public sealed class RoleWhitelistManager
{
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly IServerDbManager _db = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;

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
        SendRoleWhitelistInfo(args.Channel.UserId, args.Channel);
    }

    private void SendRoleWhitelistInfo(NetUserId userId, INetChannel channel)
    {
        var info = _cachedWhitelist.TryGetValue(userId, out var whitelist) ? whitelist : null;
        var msg = new MsgRoleWhitelistInfo() { Info = info };
        _net.ServerSendMessage(msg, channel);
    }

    private void OnDisconnect(object? sender, NetDisconnectedArgs args)
    {
        _cachedWhitelist.Remove(args.Channel.UserId);
    }

    /// <summary>
    /// Works only for connected players
    /// Extracts whitelist info from managers cache which is managed asynchroniously (because of that we can't get info for not connected players)
    /// </summary>
    public bool TryGetRoleWhitelistForPlayer(ICommonSession player, [NotNullWhen(true)] out RoleWhitelistInfo? whitelist)
    {
        return _cachedWhitelist.TryGetValue(player.UserId, out whitelist);
    }

    public bool IsAllowed(ICommonSession player, string jobId)
    {
        // we don't know what this job is and cannot allow it
        if (!_prototype.TryIndex<JobPrototype>(jobId, out var job))
        {
            return false;
        }

        // we don't know what roles are allowed for the player so we can't allow everything for it
        if (!TryGetRoleWhitelistForPlayer(player, out var whitelist))
        {
            return false;
        }

        if (job.IsWhitelisted && (whitelist.Roles.Contains(jobId) || whitelist.RolesGroups.Contains(job.WhitelistRoleGroup ?? "")))
        {
            return true;
        }

        return false;
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

    public async Task AddRoleWhitelist(NetUserId userId, string role)
    {
        await _db.AddToRoleWhitelist(userId, role);

        if (!_net.Channels.TryFirstOrDefault((channel) => channel.UserId == userId, out var channel))
            return;
        SendRoleWhitelistInfo(userId, channel);
    }

    public async Task RemoveFromRoleWhitelist(NetUserId userId, string role)
    {
        await _db.RemoveFromRoleWhitelist(userId, role);

        if (!_net.Channels.TryFirstOrDefault((channel) => channel.UserId == userId, out var channel))
            return;
        SendRoleWhitelistInfo(userId, channel);
    }

    public async Task AddRolesGroupWhitelist(NetUserId userId, string role)
    {
        await _db.AddToRoleGroupWhitelist(userId, role);

        if (!_net.Channels.TryFirstOrDefault((channel) => channel.UserId == userId, out var channel))
            return;
        SendRoleWhitelistInfo(userId, channel);
    }

    public async Task RemoveFromRolesGroupWhitelist(NetUserId userId, string role)
    {
        await _db.RemoveFromRoleGroupWhitelist(userId, role);

        if (!_net.Channels.TryFirstOrDefault((channel) => channel.UserId == userId, out var channel))
            return;
        SendRoleWhitelistInfo(userId, channel);
    }
}
