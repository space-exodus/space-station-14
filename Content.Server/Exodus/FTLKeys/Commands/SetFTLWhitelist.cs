using System.Linq;
using Content.Server.Administration;
using Content.Server.Shuttles.Systems;
using Content.Shared.Administration;
using Content.Shared.Whitelist;
using Robust.Shared.Console;
using Robust.Shared.Map;

namespace Content.Server.Exodus.FTLKey.Commands
{
    [AdminCommand(AdminFlags.Admin)]
    public sealed class SetFTLWhiteList : IConsoleCommand
    {
        [Dependency] private readonly IMapManager _mapManager = default!;
        [Dependency] private readonly IEntityManager _entity = default!;
        public string Command => "setftlwhitelist";
        public string Description => "Create White List for current FTL point";
        public string Help => "setftlwhitelist <Map Uid> <FTL Tag 1> <...>";

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            var _shuttle = _entity.System<ShuttleSystem>();

            if (shell.Player is not { } player)
            {
                shell.WriteLine("shell-server-cannot");
                return;
            }

            if (args.Length == 0)
            {
                shell.WriteLine(Loc.GetString("shell-wrong-arguments-number"));
                return;
            }

            if (!int.TryParse(args[0], out var map))
            {
                shell.WriteLine(Loc.GetString("shell-argument-must-be-number"));
                return;
            }

            var mapId = new MapId(map);

            if (!_mapManager.MapExists(mapId))
            {
                shell.WriteLine(Loc.GetString("shell-invalid-map-id"));
                return;
            }

            if (args.Length == 1)
            {
                if (_shuttle.TryAddFTLDestination(mapId, true, out var ftl))
                    _shuttle.SetFTLWhitelist((_mapManager.GetMapEntityId(mapId), ftl), null);
                return;
            }

            if (_shuttle.TryAddFTLDestination(mapId, true, out var ftlComp))
            {
                var whitelist = new EntityWhitelist()
                {
                    Tags = new List<string>(args.Skip(1))
                };

                _shuttle.SetFTLWhitelist((_mapManager.GetMapEntityId(mapId), ftlComp), whitelist);
            }
        }
    }
}
