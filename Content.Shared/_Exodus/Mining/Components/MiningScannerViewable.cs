// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt
// Based on https://github.com/space-wizards/space-station-14/tree/f9320aacd72315a38ddb9a4d73dbd6231a8c1db4/Content.Shared/Mining/Components/MiningScannerViewable.cs

using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Exodus.Mining.Components;

[RegisterComponent, NetworkedComponent, Access(typeof(SharedMiningScannerViewerSystem))]
public sealed partial class MiningScannerViewableComponent : Component;

[Serializable, NetSerializable]
public enum MiningScannerVisualLayers : byte
{
    Overlay
}
