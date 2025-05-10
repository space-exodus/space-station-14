// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt
// Based on https://github.com/space-wizards/space-station-14/tree/f9320aacd72315a38ddb9a4d73dbd6231a8c1db4/Content.Shared/Mining/Components/MiningScannerViewerComponent.cs

using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Exodus.Mining.Components;

[RegisterComponent, NetworkedComponent, Access(typeof(SharedMiningScannerViewerSystem))]
public sealed partial class MiningScannerViewerComponent : Component
{
    [DataField]
    public List<MiningScannerRecord> Records = new();
}

[DataDefinition, Serializable, NetSerializable]
public sealed partial class MiningScannerRecord
{
    [DataField]
    public MapCoordinates PingLocation;

    [DataField]
    public float ViewRange;

    [DataField]
    public TimeSpan CreatedAt;

    [DataField]
    public TimeSpan AnimationDuration = TimeSpan.FromSeconds(1.5f);

    /// <summary>
    /// How long ores should be showing minus the duration of the animation
    /// </summary>
    [DataField]
    public TimeSpan Delay = TimeSpan.FromSeconds(3.5f);
}

[Serializable, NetSerializable]
public sealed partial class MiningScannerViewerComponentState : ComponentState
{
    [DataField]
    public List<MiningScannerRecord> Records = new();
}
