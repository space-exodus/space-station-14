namespace Content.Shared.Exodus.Abilities;

[RegisterComponent]
public sealed partial class AbilitiesComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite), DataField("actions")]
    public List<string> Actions = [];
}
