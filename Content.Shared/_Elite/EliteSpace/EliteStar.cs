using System.Numerics;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Elite.EliteSpace;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class EliteStarComponent : Component
{
    [AutoNetworkedField]
    public HashSet<EntityUid> Objects = new();

    [AutoNetworkedField]
    public HashSet<BluespaceCorridor> Corridors = new();

    [DataField, AutoNetworkedField]
    public ProtoId<EliteStarPrototype> Proto;
}

public struct BluespaceCorridor
{
    public EntityUid From;
    public EntityUid To;
    public bool OneWay;
}

[Prototype]
public sealed partial class EliteStarPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = string.Empty;

    [DataField(required: true)]
    public LocId Name;

    [DataField(required: true)]
    public Vector2 Position;

    [DataField(required: true)]
    public HashSet<ProtoId<EliteObjectPrototype>> Objects = new();

    [DataField(required: true)]
    public HashSet<BluespaceCorridorDefinition> Corridors = new();

    [DataField]
    public ProtoId<EliteFactionPrototype>? Faction;
}

/// <remarks>
/// Used only at load
/// </remarks>
[DataDefinition]
public partial struct BluespaceCorridorDefinition
{
    [DataField]
    public ProtoId<EliteObjectPrototype> From;

    [DataField]
    public ProtoId<EliteObjectPrototype> To;

    [DataField]
    public bool OneWay;
}
