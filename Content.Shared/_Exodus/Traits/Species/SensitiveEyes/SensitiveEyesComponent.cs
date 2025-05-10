// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;

namespace Content.Shared.Exodus.Traits.Species.SensitiveEyes;

[RegisterComponent, NetworkedComponent]
public sealed partial class SensitiveEyesComponent : Component
{
    [DataField]
    public float DurationModifier = 1.2f;
}
