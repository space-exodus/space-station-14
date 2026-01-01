using Content.Shared.Elite.EliteSpace;
using Robust.Server.GameObjects;
using Robust.Shared.EntitySerialization.Systems;
using Robust.Shared.Map.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server.Elite.EliteSpace;

/// <inheritdoc />
public sealed partial class EliteSpaceSystem : SharedEliteSpaceSystem
{
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly MapLoaderSystem _mapLoader = default!;
    [Dependency] private readonly MapSystem _map = default!;
    [Dependency] private readonly MetaDataSystem _metadata = default!;

    public void LoadSector(ProtoId<EliteSectorMapPrototype> sectorProtoId)
    {
        if (!_proto.TryIndex(sectorProtoId, out var sectorProto))
        {
            DebugTools.Assert($"Tried to load an invalid sector map prototype: {sectorProtoId}");
            return;
        }

        var sectorEnt = Spawn(null);
        var sector = new EliteSectorMapComponent()
        {
            Sector = sectorProtoId,
        };
        AddComp(sectorEnt, sector);

        foreach (var starProto in sectorProto.Stars)
        {
            var star = LoadStarSystem(starProto);
            sector.Stars.Add(star);
        }

        foreach (var corridorDef in sectorProto.StarCorridors)
        {
            var corridor = new StarBluespaceCorridor()
            {
                From = GetStar((sectorEnt, sector), corridorDef.From),
                To = GetStar((sectorEnt, sector), corridorDef.To),
            };
            sector.StarCorridors.Add(corridor);
        }
    }

    public EntityUid GetStar(Entity<EliteSectorMapComponent> sector, ProtoId<EliteStarPrototype> id)
    {
        foreach (var star in sector.Comp.Stars)
        {
            var starComp = Comp<EliteStarComponent>(star);
            if (starComp.Proto == id)
                return star;
        }
        throw new ArgumentException($"Tried to get not existing star with id {id}");
    }

    public EntityUid LoadStarSystem(ProtoId<EliteStarPrototype> starProtoId)
    {
        if (!_proto.TryIndex(starProtoId, out var starProto))
        {
            DebugTools.Assert($"Tried to load an invalid sector map prototype: {starProtoId}");
            return EntityUid.Invalid;
        }

        var starEnt = Spawn(null);
        var star = new EliteStarComponent()
        {
            Proto = starProtoId,
        };
        AddComp(starEnt, star);

        foreach (var objProtoId in starProto.Objects)
        {
            LoadSystemObject(starEnt, objProtoId);
        }

        foreach (var corridorDef in starProto.Corridors)
        {
            var corridor = new BluespaceCorridor()
            {
                From = GetObject((starEnt, star), corridorDef.From),
                To = GetObject((starEnt, star), corridorDef.To),
                OneWay = corridorDef.OneWay,
            };
            star.Corridors.Add(corridor);
        }

        return starEnt;
    }

    public EntityUid GetObject(Entity<EliteStarComponent> star, ProtoId<EliteObjectPrototype> id)
    {
        foreach (var obj in star.Comp.Objects)
        {
            var objComp = Comp<EliteObjectComponent>(obj);
            if (objComp.Proto == id)
                return obj;
        }

        throw new ArgumentException($"Tried to get not existing object with id {id}");
    }

    public EntityUid LoadSystemObject(
        Entity<EliteStarComponent?> star,
        ProtoId<EliteObjectPrototype> objectProtoId,
        bool loadMapImmediately = false
    )
    {
        if (!Resolve(star, ref star.Comp))
            return EntityUid.Invalid;

        if (!_proto.TryIndex(objectProtoId, out var objectProto))
        {
            DebugTools.Assert($"Tried to load an invalid elite object prototype: {objectProtoId}");
            return EntityUid.Invalid;
        }

        var objEnt = _map.CreateUninitializedMap();
        _metadata.SetEntityName(objEnt, objectProto.Name);
        var obj = new EliteObjectComponent()
        {
            Proto = objectProtoId,
            Star = star.Owner,
            Coordinates = objectProto.Coordinates,
        };
        AddComp(objEnt, obj);

        if (objectProto.LoadMapAtStart || loadMapImmediately)
            LoadObjectMap((objEnt, obj));

        return objEnt;
    }

    public void LoadObjectMap(Entity<EliteObjectComponent> objectUid, EliteObjectPrototype? prototype = null)
    {
        if (prototype == null)
        {
            if (!_proto.TryIndex(objectUid.Comp.Proto, out prototype))
            {
                DebugTools.Assert($"Tried to load an invalid elite object prototype: {objectUid.Comp.Proto}");
                return;
            }
        }

        if (prototype.MapPath is not { } mapPath)
            return;

        var map = Comp<MapComponent>(objectUid);
        _mapLoader.TryMergeMap(map.MapId, mapPath, out _);
    }

    // public void RestrictObjectCorridors(EntityUid objectUid);

    // public IEnumerable<BluespaceRoute> FindRoutes(
    //     EntityUid fromObject,
    //     EntityUid toObject,
    //     BluespaceRouteConstraints constraints = default
    // );

    // public bool IsCorridorAccessible(EntityUid fromObject, EntityUid toObject);

    // public EntityUid SpawnDynamicObject(
    //     EntityUid starUid,
    //     EliteDynamicObjectArgs parameters
    // );
}

// public struct EliteDynamicObjectArgs
// {
//     public EliteDynamicObjectArgs(string name, SystemObjectType type, PolarCoordinates coordinates, ResPath? mapPath, bool isTemporary, TimeSpan? lifetime = null)
//     {
//         Name = name;
//         Type = type;
//         Coordinates = coordinates;
//         MapPath = mapPath;
//         IsTemporary = isTemporary;
//         Lifetime = lifetime;
//     }

//     public required string Name { get; set; }
//     public required SystemObjectType Type { get; set; }
//     public PolarCoordinates Coordinates { get; set; }
//     public ResPath? MapPath { get; set; }
//     public bool IsTemporary { get; set; } = true;
//     public TimeSpan? Lifetime { get; set; }
// }
