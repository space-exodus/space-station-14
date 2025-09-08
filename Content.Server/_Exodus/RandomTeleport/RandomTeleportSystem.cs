// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt
using System.Linq;
using Robust.Shared.Map;
using Robust.Shared.Random;
using Robust.Shared.Containers;
using Content.Shared.Maps;

namespace Content.Server.Exodus.RandomTeleport;

public partial class RandomTeleportSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedTransformSystem _xform = default!;
    [Dependency] private readonly EntityLookupSystem _entityLookup = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly SharedMapSystem _mapSystem = default!;

    private const int MaxRandomTeleportAttempts = 20;

    public EntityCoordinates? GetRandomCoordinates(EntityUid uid, float range, bool spaceAllowed = true)
    {
        if (_container.IsEntityOrParentInContainer(uid))
            return null;

        var xform = Transform(uid);
        var coord = xform.Coordinates;
        var newCoords = coord.Offset(_random.NextVector2(range));

        bool notValidCoordinates = false;

        for (int i = 0; i < MaxRandomTeleportAttempts; i++)
        {
            var randVector = _random.NextVector2(range);
            newCoords = coord.Offset(randVector);

            if (!spaceAllowed)
            {
                if (!_mapManager.TryFindGridAt(_xform.ToMapCoordinates(newCoords), out var gridUid, out var grid) || !_mapSystem.TryGetTileRef(gridUid, grid, newCoords, out var tileRef))
                    continue;

                if (tileRef.Tile.IsSpace())
                    continue;
            }

            if (!_entityLookup.GetEntitiesIntersecting(_xform.ToMapCoordinates(newCoords), LookupFlags.Static).Any())
            {
                notValidCoordinates = true;
                break;
            }
        }

        return notValidCoordinates ? newCoords : null;
    }
}
