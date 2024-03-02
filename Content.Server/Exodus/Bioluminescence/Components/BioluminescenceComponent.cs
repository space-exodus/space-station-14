using Content.Shared.Actions;

namespace Content.Server.Exodus.Bioluminescence;

[RegisterComponent]
public sealed partial class BioluminescenceComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite), DataField("action")]
    public string Action = "TurnBioluminescenceAction";
}

