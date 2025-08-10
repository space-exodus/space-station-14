// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt
using Robust.Shared.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Utility;

namespace Content.Shared.Exodus.Stealth.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ToggleStealthComponent : Component
{
    [DataField]
    public bool Enabled;

    [DataField]
    public bool Parent = false;

    [DataField]
    public StealthData Stealth = new();

    [DataField, AutoNetworkedField]
    public EntityUid Target;
}
