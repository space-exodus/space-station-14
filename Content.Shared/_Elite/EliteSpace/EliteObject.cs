using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Elite.EliteSpace;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class EliteObjectComponent : Component
{
    [DataField, AutoNetworkedField]
    public ProtoId<EliteObjectPrototype> Proto;

    [DataField, AutoNetworkedField]
    public EntityUid Star;

    [DataField, AutoNetworkedField]
    public PolarCoordinates Coordinates;
}

[Prototype]
public sealed partial class EliteObjectPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = string.Empty;

    [DataField(required: true)]
    public LocId Name;

    [DataField]
    public ResPath? MapPath;

    [DataField(required: true)]
    public SystemObjectType Type;

    [DataField]
    public bool LoadMapAtStart;

    [DataField(required: true)]
    public PolarCoordinates Coordinates;
}

public enum SystemObjectType
{
    Star,
    Planet,
    GasGiant,
    Moon,
    AsteroidBelt,
    Station,
    Outpost,
    Anomaly,
    Wormhole,
    DebrisField
}
