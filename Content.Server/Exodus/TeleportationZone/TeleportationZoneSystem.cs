using Content.Server.Atmos.EntitySystems;
using Content.Server.Chat.Systems;
using Content.Server.Exodus.TeleportationZone.Console;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Coordinates;
using Content.Shared.Exodus.TeleportationZone;
using Content.Shared.Maps;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

namespace Content.Server.Exodus.TeleportationZone
{
    public sealed class TeleportationZoneSystem : EntitySystem
    {
        [Dependency] private readonly AtmosphereSystem _atmosphere = default!;
        [Dependency] private readonly EntityLookupSystem _lookupSystem = default!;
        [Dependency] private readonly IEntityManager _entMan = default!;
        [Dependency] private readonly ITileDefinitionManager _tileDefinitionManager = default!;
        [Dependency] private readonly SharedMapSystem _mapSystem = default!;
        [Dependency] private readonly SharedTransformSystem _transform = default!;
        [Dependency] private readonly StationSystem _station = default!;


        public void CheckTils(EntityUid console, TeleportationZoneConsoleComponent component, EntityUid start_station_uid, MapGridComponent start_grid, EntityUid end_station_uid, MapGridComponent end_grid, EntityCoordinates pos)
        {
            component.left_border -= 0.5f;
            component.right_border -= 0.5f;
            component.top_border -= 0.5f;
            component.bottom_border -= 0.5f;

            for (int i = (int) component.left_border + 1; i < (int) component.right_border; i++)
            {
                for (int j = (int) component.top_border - 1; j > (int) component.bottom_border; j--)
                {
                    var coords = new Vector2i(i, j);

                    if (!start_grid.TryGetTileRef(coords, out var tileRef))
                        continue;

                    int dX = i - ((int) component.left_border + 1);
                    int dY = j - ((int) component.top_border - 1);

                    var TileType = tileRef.Tile.GetContentTileDefinition().ID;
                    CreateFloor(end_station_uid, end_grid, pos, dX, dY, (string) TileType);

                    foreach (var entity in _lookupSystem.GetLocalEntitiesIntersecting(start_station_uid, coords))
                    {
                        if (_entMan.TryGetComponent(entity, out TransformComponent? transComp))
                        {
                            var parent = transComp.ParentUid;

                            if (parent == start_station_uid)
                            {
                                if (!EnoughMatter(console, component, entity))
                                    continue;

                                CreateEntity(end_station_uid, end_grid, pos, dX, dY, entity, transComp, transComp.Anchored, transComp.LocalRotation);
                            }
                        }
                    }
                    CreatePlatingFloor(start_station_uid, start_grid, i, j);
                    CheckRemainingMatter(console, component, start_station_uid, start_grid);
                }
            }
        }

        private bool EnoughMatter(EntityUid console, TeleportationZoneConsoleComponent component, EntityUid entity)
        {
            if (TryComp<TeleportationZoneListOfPricesComponent>(console, out var pricesComp))
            {
                if (!TryComp<MetaDataComponent>(entity, out var mdComp))
                    return true;

                if (pricesComp.ListOfPrices.TryGetValue(mdComp.EntityPrototype!.ID!, out var value))
                {
                    if (component.RealMatter < value)
                        return false;

                    component.RealMatter -= value;
                    return true;
                }
                return true;
            }
            return true;
        }

        private void CreatePlatingFloor(EntityUid start_station_uid, MapGridComponent start_gridComp, int X, int Y)
        {
            var tile = new Vector2i(X, Y);
            var new_pos = _mapSystem.GridTileToLocal(start_station_uid, start_gridComp, tile);
            var plating = _tileDefinitionManager["Plating"];
            start_gridComp.SetTile(new_pos, new Tile(plating.TileId));
        }

        private void CreateFloor(EntityUid end_station_uid, MapGridComponent end_gridComp, EntityCoordinates pos, int dX, int dY, string TileType)
        {
            if (TileType.Equals("Plating"))
                return;

            int coordX = (int) (pos.X - 0.5f) + dX;
            int coordY = (int) (pos.Y - 0.5f) + dY;
            var tile = new Vector2i(coordX, coordY);
            var tile2 = new Vector2i(coordX + 1, coordY + 1);
            foreach (var entity in _lookupSystem.GetLocalEntitiesIntersecting(end_station_uid, tile))
            {
                _transform.SetCoordinates(entity, _mapSystem.GridTileToLocal(end_station_uid, end_gridComp, tile2));
            }

            var new_pos = _mapSystem.GridTileToLocal(end_station_uid, end_gridComp, tile);
            var plating = _tileDefinitionManager[TileType];
            end_gridComp.SetTile(new_pos, new Tile(plating.TileId));
        }

        private void CreateEntity(EntityUid end_station_uid, MapGridComponent end_gridComp, EntityCoordinates pos, int dX, int dY, EntityUid entity, TransformComponent transComp, bool isAnch, Angle rot)
        {
            int coordX = (int) (pos.X - 0.5f) + dX;
            int coordY = (int) (pos.Y - 0.5f) + dY;
            var tile = new Vector2i(coordX, coordY);
            var new_pos = _mapSystem.GridTileToLocal(end_station_uid, end_gridComp, tile);
            _transform.SetCoordinates(entity, new_pos);
            transComp.Anchored = isAnch; // objects (e.g. walls) lose their attachment to the floor during teleportation
            transComp.LocalRotation = rot; // this is necessary so that the object is correctly rotated relative to the new station 
        }

        private void CheckRemainingMatter(EntityUid uid, TeleportationZoneConsoleComponent component, EntityUid start_station_uid, MapGridComponent start_grid_comp)
        {
            if (component.RealMatter == 0)
                return;

            Random rnd = new Random();
            var coords = Transform(uid).Coordinates;
            for (int i = 0; i < component.RealMatter; i += 10)
            {
                while (true)
                {
                    int radius = rnd.Next(-30, 30);
                    int X = (int)(coords.X - 0.5f) + radius;
                    radius = rnd.Next(-30, 30);
                    int Y = (int)(coords.Y - 0.5f) + radius;

                    var tile = new Vector2i(X, Y);
                    if (_atmosphere.IsTileSpace(start_station_uid, Transform(uid).MapUid, tile))
                    {
                        continue;
                    }

                    var pos = _mapSystem.GridTileToLocal(start_station_uid, start_grid_comp, tile);
                    EntityManager.SpawnEntity("AnomalyBluespace", pos);
                    break;
                }
            }
            component.RealMatter = 0;
        }
    }
}
