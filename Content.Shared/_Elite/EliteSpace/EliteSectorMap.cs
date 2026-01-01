using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Elite.EliteSpace;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class EliteSectorMapComponent : Component
{
    [DataField(required: true), AutoNetworkedField]
    public ProtoId<EliteSectorMapPrototype> Sector;

    [AutoNetworkedField]
    public HashSet<EntityUid> Stars = new();

    [AutoNetworkedField]
    public HashSet<StarBluespaceCorridor> StarCorridors = new();
}

public struct StarBluespaceCorridor
{
    public EntityUid From;
    public EntityUid To;
}

[Prototype]
public sealed partial class EliteSectorMapPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = string.Empty;

    [DataField(required: true)]
    public HashSet<ProtoId<EliteStarPrototype>> Stars = new();

    [DataField(required: true)]
    public HashSet<StarBluespaceCorridorDefinition> StarCorridors = new();
}

[DataDefinition]
public partial struct StarBluespaceCorridorDefinition
{
    [DataField]
    public ProtoId<EliteStarPrototype> From;

    [DataField]
    public ProtoId<EliteStarPrototype> To;
}
