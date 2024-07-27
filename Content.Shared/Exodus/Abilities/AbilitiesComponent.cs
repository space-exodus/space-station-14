using Content.Shared.FixedPoint;

namespace Content.Shared.Exodus.Abilities;

[RegisterComponent]
public sealed partial class AbilitiesComponent : Component
{
    [DataField("grantActions")]
    public List<string> GrantActions = [];

    [ViewVariables(VVAccess.ReadWrite)]
    public Dictionary<string, EntityUid> Actions = [];

    /// <summary>
    /// Depending on the amount of damage, changes the available abilities.
    /// </summary>
    [DataField("actionsThresholds")]
    public Dictionary<FixedPoint2, List<string>> ActionsThresholds = [];
}
