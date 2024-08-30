using Content.Server.Administration;
using Content.Server.Database;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.Exodus.Sponsors.Commands
{
    [AdminCommand(AdminFlags.Sponsors)]
    public sealed class PromoteSponsorCommand : IConsoleCommand
    {
        [Dependency] private readonly IServerDbManager _db = default!;

        public string Command => "promotesponsor";
        public string Description => "Makes someone a sponsor";
        public string Help => $"Usage: {Command} <CKey>";

        public async void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (args.Length != 1)
            {
                shell.WriteLine("Need only one argument.");
                return;
            }

            var playerRecord = await _db.GetPlayerRecordByUserName(args[0]);

            if (playerRecord == null)
            {
                shell.WriteLine("Cannot find specified player in database.");
                return;
            }

            if (playerRecord.IsPremium)
            {
                shell.WriteLine("Specified player is already a sponsor.");
                return;
            }

            await _db.PromoteSponsor(playerRecord.UserId);
            shell.WriteLine($"Promoted {args[0]} to sponsors");
        }
    }

    [AdminCommand(AdminFlags.Sponsors)]
    public sealed class UnpromoteSponsorCommand : IConsoleCommand
    {
        [Dependency] private readonly IServerDbManager _db = default!;

        public string Command => "unpromotesponsor";
        public string Description => "Removes sponsorship from someone";
        public string Help => $"Usage: {Command} <CKey>";

        public async void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (args.Length != 1)
            {
                shell.WriteLine("Need only one argument.");
                return;
            }

            var playerRecord = await _db.GetPlayerRecordByUserName(args[0]);

            if (playerRecord == null)
            {
                shell.WriteLine("Cannot find specified player in database.");
                return;
            }

            if (!playerRecord.IsPremium)
            {
                shell.WriteLine("Specified player is already not a sponsor.");
                return;
            }

            await _db.UnpromoteSponsor(playerRecord.UserId);
            shell.WriteLine($"Removed sponsorship from {args[0]}");
        }
    }
}
