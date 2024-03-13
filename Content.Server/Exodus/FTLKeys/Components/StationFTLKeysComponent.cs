namespace Content.Server.Shuttles.Components;

/// <summary>
/// Add base station FTL Tag "Station"
/// </summary>
[RegisterComponent]
public sealed partial class StationFTLKeysComponent : Component
{
    public List<string> StationTags = ["FTLDestinationAccessStation"];
}
