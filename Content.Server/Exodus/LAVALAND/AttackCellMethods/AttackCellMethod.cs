using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Utility;
using Robust.Shared.Serialization;
using System.Numerics;

using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Reflection;
using Robust.Shared.Replays;
using Robust.Shared.Map;

namespace Content.Server.Exodus.MassSpawn.Methods;

[DataDefinition]
public abstract partial class AttackCellMethod
{
    protected SharedTransformSystem? _transform = null;
    protected EntityManager? _entityManager = null;

    protected EntityUid OwnerUid = EntityUid.Invalid;
    protected MassSpawnerComponent? OwnerComp;
    protected MassSpawnGroup? OwnerGroup;
    protected EntityCoordinates HeartPositionCoords = EntityCoordinates.Invalid;

    public AttackCeilMethodStage LifeStage = AttackCeilMethodStage.BeforeStart;

    public abstract AttackCellMethod Copy();

    public virtual void MethodInitialize(EntityUid uid, MassSpawnerComponent comp, MassSpawnGroup group, SharedTransformSystem transform, EntityManager entManager)
    {
        _transform = transform;
        _entityManager = entManager;

        OwnerUid = uid;
        OwnerComp = comp;
        OwnerGroup = group;

        OwnerGroup.SpawnProtoID ??= OwnerComp.SpawnProtoID;

        if (!_transform.TryGetMapOrGridCoordinates(OwnerUid, out var coordinates))
            return;

        HeartPositionCoords = coordinates.Value.Offset(group.Offset);
    }

    public virtual void Start()
    {
        LifeStage = AttackCeilMethodStage.Active;
    }

    public virtual void Update(TimeSpan curTime)
    {
        LifeStage = AttackCeilMethodStage.Dead;
    }
}
public enum AttackCeilMethodStage
{
    BeforeStart,
    Active,
    Dead
}
