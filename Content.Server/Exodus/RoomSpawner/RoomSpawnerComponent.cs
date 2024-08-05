using Content.Shared.Random;
using Content.Shared.Tag;
using Robust.Shared.Prototypes;

namespace Content.Server.Exodus.RoomSpawner;

/// <summary>
/// allows you to spawn one of the rooms during initialization
/// </summary>
[RegisterComponent, Access(typeof(ExodusRoomSpawnerSystem))]
public sealed partial class ExodusRoomSpawnerComponent : Component
{
    [DataField(required: true)]
    public HashSet<ProtoId<TagPrototype>> RoomsTag = new();

    [DataField]
    public float Prob = 1f;

    [DataField]
    public bool Rotation = true;

    [DataField]
    public bool ClearExisting = true;
}
