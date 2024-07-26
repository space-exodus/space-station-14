using Content.Server.Exodus.MassSpawn.Methods;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Utility;
using Robust.Shared.Serialization;
using Robust.Shared.Map;
using System.Numerics;
using System.Reflection;

namespace Content.Server.Exodus.MassSpawn;

[RegisterComponent]
public sealed partial class MassSpawnerComponent : Component
{
    [DataField("removeAfterUsed")]
    public bool RemoveAfterUsed = true;

    [DataField("spawnProtoID", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string SpawnProtoID = "AttackCellUnitDEBUG";

    [DataField("groups")]
    public List<MassSpawnGroup> Groups = [];

    [Access(typeof(MassSpawnerSystem))]
    public List<MassSpawnGroup> AddingGroups = [];

    [Access(typeof(MassSpawnerSystem))]
    public List<MassSpawnGroup> RemovingGroups = [];

    public void AddGroup(MassSpawnGroup group)
    {
        AddingGroups.Add(group);
    }
    public void AddGroup(List<MassSpawnGroup> group)
    {
        AddingGroups.AddRange(group);
    }

    public void RemoveGroup(MassSpawnGroup group)
    {
        RemovingGroups.Add(group);
    }
    public void RemoveGroup(List<MassSpawnGroup> group)
    {
        RemovingGroups.AddRange(group);
    }

}

[DataDefinition]
public sealed partial class MassSpawnGroup
{
    [DataField("offset")]
    public Vector2 Offset = Vector2.Zero;

    [DataField("spawnProtoID", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string? SpawnProtoID = null;

    [DataField("delay")]
    public float DelayBehindStart = 0;

    [DataField("groupProto", customTypeSerializer: typeof(PrototypeIdSerializer<MassSpawnGroupPrototype>))]
    public string? GroupProto;

    [DataField("method")]
    public AttackCellMethod? Method;


    public bool GroupInit = false;

    public TimeSpan GroupInitTime = new();

    public void CopyFrom(MassSpawnGroup groupFrom)
    {
        Offset += groupFrom.Offset;
        SpawnProtoID ??= groupFrom.SpawnProtoID;
        DelayBehindStart += groupFrom.DelayBehindStart;
        GroupProto = groupFrom.GroupProto;

        if (groupFrom.Method != null)
            Method = groupFrom.Method.Copy();
    }
}

[Prototype("spawnGroupProto")]
public sealed partial class MassSpawnGroupPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField("group", required: true)]
    public MassSpawnGroup Group = new();
}
