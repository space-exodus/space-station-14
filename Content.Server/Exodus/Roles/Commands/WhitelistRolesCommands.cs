using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.Roles;

[AdminCommand(AdminFlags.Sponsors)]
public sealed class AddWhitelistRoleCommand : LocalizedCommands
{
    public override string Command => "whitelistroleadd";

    public override async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length < 2)
        {
            shell.WriteError(Loc.GetString("shell-need-minimum-arguments", ("minimum", 2)));
            shell.WriteLine(Help);
            return;
        }

        var roleWhitelist = IoCManager.Resolve<RoleWhitelistManager>();
        var loc = IoCManager.Resolve<IPlayerLocator>();

        var name = args[0];
        var role = args[1];
        var data = await loc.LookupIdByNameAsync(name);

        if (data != null)
        {
            var guid = data.UserId;
            var whitelist = await roleWhitelist.GetRoleWhitelistInfo(guid);
            var isWhitelisted = whitelist.Roles.Contains(role);

            if (isWhitelisted)
            {
                shell.WriteLine(Loc.GetString("cmd-whitelistadd-existing", ("username", data.Username)));
                return;
            }

            await roleWhitelist.AddRoleWhitelist(guid, role);
            shell.WriteLine(Loc.GetString("cmd-whitelistadd-added", ("username", data.Username)));
            return;
        }

        shell.WriteError(Loc.GetString("cmd-whitelistadd-not-found", ("username", args[0])));
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
        {
            return CompletionResult.FromHint(Loc.GetString("cmd-whitelistadd-arg-player"));
        }

        return CompletionResult.Empty;
    }
}

[AdminCommand(AdminFlags.Sponsors)]
public sealed class RemoveFromRoleWhitelistCommand : LocalizedCommands
{
    public override string Command => "whitelistroleremove";

    public override async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length < 2)
        {
            shell.WriteError(Loc.GetString("shell-need-minimum-arguments", ("minimum", 2)));
            shell.WriteLine(Help);
            return;
        }

        var roleWhitelist = IoCManager.Resolve<RoleWhitelistManager>();
        var loc = IoCManager.Resolve<IPlayerLocator>();

        var name = args[0];
        var role = args[1];
        var data = await loc.LookupIdByNameAsync(name);

        if (data != null)
        {
            var guid = data.UserId;
            var whitelist = await roleWhitelist.GetRoleWhitelistInfo(guid);
            var isWhitelisted = whitelist.Roles.Contains(role);
            if (!isWhitelisted)
            {
                shell.WriteLine(Loc.GetString("cmd-whitelistremove-existing", ("username", data.Username)));
                return;
            }

            await roleWhitelist.RemoveFromRoleWhitelist(guid, role);
            shell.WriteLine(Loc.GetString("cmd-whitelistremove-removed", ("username", data.Username)));
            return;
        }

        shell.WriteError(Loc.GetString("cmd-whitelistremove-not-found", ("username", args[0])));
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
        {
            return CompletionResult.FromHint(Loc.GetString("cmd-whitelistremove-arg-player"));
        }

        return CompletionResult.Empty;
    }
}

[AdminCommand(AdminFlags.Sponsors)]
public sealed class AddWhitelistRolesGroupCommand : LocalizedCommands
{
    public override string Command => "whitelistrolesgroupadd";

    public override async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length < 2)
        {
            shell.WriteError(Loc.GetString("shell-need-minimum-arguments", ("minimum", 2)));
            shell.WriteLine(Help);
            return;
        }

        var roleWhitelist = IoCManager.Resolve<RoleWhitelistManager>();
        var loc = IoCManager.Resolve<IPlayerLocator>();

        var name = args[0];
        var group = args[1];
        var data = await loc.LookupIdByNameAsync(name);

        if (data != null)
        {
            var guid = data.UserId;
            var whitelist = await roleWhitelist.GetRoleWhitelistInfo(guid);
            var isWhitelisted = whitelist.RolesGroups.Contains(group);

            if (isWhitelisted)
            {
                shell.WriteLine(Loc.GetString("cmd-whitelistadd-existing", ("username", data.Username)));
                return;
            }

            await roleWhitelist.AddRolesGroupWhitelist(guid, group);
            shell.WriteLine(Loc.GetString("cmd-whitelistadd-added", ("username", data.Username)));
            return;
        }

        shell.WriteError(Loc.GetString("cmd-whitelistadd-not-found", ("username", args[0])));
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
        {
            return CompletionResult.FromHint(Loc.GetString("cmd-whitelistadd-arg-player"));
        }

        return CompletionResult.Empty;
    }
}

[AdminCommand(AdminFlags.Sponsors)]
public sealed class RemoveFromRolesGroupWhitelistCommand : LocalizedCommands
{
    public override string Command => "whitelistrolesgroupremove";

    public override async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length < 2)
        {
            shell.WriteError(Loc.GetString("shell-need-minimum-arguments", ("minimum", 2)));
            shell.WriteLine(Help);
            return;
        }

        var roleWhitelist = IoCManager.Resolve<RoleWhitelistManager>();
        var loc = IoCManager.Resolve<IPlayerLocator>();

        var name = args[0];
        var group = args[1];
        var data = await loc.LookupIdByNameAsync(name);

        if (data != null)
        {
            var guid = data.UserId;
            var whitelist = await roleWhitelist.GetRoleWhitelistInfo(guid);
            var isWhitelisted = whitelist.RolesGroups.Contains(group);
            if (!isWhitelisted)
            {
                shell.WriteLine(Loc.GetString("cmd-whitelistremove-existing", ("username", data.Username)));
                return;
            }

            await roleWhitelist.RemoveFromRolesGroupWhitelist(guid, group);
            shell.WriteLine(Loc.GetString("cmd-whitelistremove-removed", ("username", data.Username)));
            return;
        }

        shell.WriteError(Loc.GetString("cmd-whitelistremove-not-found", ("username", args[0])));
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
        {
            return CompletionResult.FromHint(Loc.GetString("cmd-whitelistremove-arg-player"));
        }

        return CompletionResult.Empty;
    }
}
