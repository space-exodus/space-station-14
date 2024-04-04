using System.Diagnostics.CodeAnalysis;
using Content.Shared.Roles;
using Robust.Shared.Network;
using Robust.Shared.Utility;

namespace Content.Client.Roles;

public sealed class RoleWhitelistManager
{
    [Dependency] private readonly IClientNetManager _net = default!;
    private RoleWhitelistInfo? _info;

    public void Initialize()
    {
        _net.RegisterNetMessage<MsgRoleWhitelistInfo>(OnMessage);
    }

    public void OnMessage(MsgRoleWhitelistInfo message)
    {
        _info = message.Info;
    }

    public bool TryGetInfo([NotNullWhen(true)] out RoleWhitelistInfo? whitelist)
    {
        whitelist = _info;
        return _info != null;
    }

    public bool HasRole(string role)
    {
        TryGetInfo(out var whitelist);
        DebugTools.Assert(whitelist is not null, "Cannot get user's roles whitelist");
        return whitelist.Roles.Contains(role);
    }

    public bool HasRoleGroup(string group)
    {
        TryGetInfo(out var whitelist);
        DebugTools.Assert(whitelist is not null, "Cannot get user's roles whitelist");
        return whitelist.RolesGroups.Contains(group);
    }

    public bool HasJob(JobPrototype job)
    {
        // is whitelisted, has role or has role group
        return job.IsWhitelisted &&
                (HasRole(job.ID) || job.WhitelistRoleGroup is not null &&
                HasRoleGroup(job.WhitelistRoleGroup));
    }
}
