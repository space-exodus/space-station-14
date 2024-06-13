using Content.Shared.Actions;

namespace Content.Server.Exodus.Lavaland;

[RegisterComponent]
public sealed partial class AbilityAttackComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite), DataField("actions")]
    public List<string> Actions = [];
}
