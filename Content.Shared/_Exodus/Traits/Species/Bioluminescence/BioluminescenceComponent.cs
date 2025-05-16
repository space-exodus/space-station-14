// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Exodus.Traits.Species.Bioluminescence;

[RegisterComponent, NetworkedComponent]
public sealed partial class BioluminescenceComponent : Component
{
    [DataField]
    public EntProtoId ToggleActionPrototype = "ActionToggleBioluminescence";

    [DataField]
    public EntityUid? ActionEntity;

    [DataField]
    public Color Color = Color.Yellow;
}
