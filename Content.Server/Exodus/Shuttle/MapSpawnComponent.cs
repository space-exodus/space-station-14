using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server.Shuttles.Systems;

/// <summary>
/// Similar to <see cref="MapSpawnComponent"/> except spawns the grid on new map
/// </summary>
[RegisterComponent, Access(typeof(ShuttleSystem))]
public sealed partial class MapSpawnComponent : Component
{
    /// <summary>
    /// Dictionary of groups where each group will have entries selected.
    /// String is just an identifier to make yaml easier.
    /// </summary>
    [DataField("maps", required: true)] public List<MapSpawnGroup> Groups = [];
}

[DataRecord]
public record struct MapSpawnGroup
{
    public string MapName = "map";

    public List<ResPath> Paths = [];

    public int MinCount = 1;

    public int MaxCount = 1;

    /// <summary>
    /// Components to be added to map.
    /// </summary>
    public ComponentRegistry AddComponents = new();

    /// <summary>
    /// Hide the IFF label of the grid.
    /// </summary>
    public bool Hide = false;

    /// <summary>
    /// Should we set the metadata name of a grid. Useful for admin purposes.
    /// </summary>

    public MapSpawnGroup()
    {
    }
}
