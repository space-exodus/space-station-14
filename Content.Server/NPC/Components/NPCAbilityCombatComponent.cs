// Exodus-Lavaland
namespace Content.Server.NPC.Components;

/// <summary>
/// Added to NPCs whenever they're in ability combat so they can be handled by the dedicated system.
/// </summary>
[RegisterComponent]
public sealed partial class NPCAbilityCombatComponent : Component
{
    [ViewVariables]
    public EntityUid Target;

    [ViewVariables]
    public AbilityCombatStatus Status = AbilityCombatStatus.Normal;

    [ViewVariables]
    public TimeSpan NextAction = new();

    [ViewVariables]
    public int ActionsPerUpd = 1;

    [ViewVariables]
    public int UsedActionsLastUpd = 0;

    [ViewVariables]
    public float ActionsTimeReload = 1.0f;

}

public enum AbilityCombatStatus : byte
{
    /// <summary>
    /// The target isn't in LOS anymore.
    /// </summary>
    NotInSight,

    /// <summary>
    /// Due to some generic reason we are unable to attack the target.
    /// </summary>
    Unspecified,

    /// <summary>
    /// Set if we can't reach the target for whatever reason.
    /// </summary>
    TargetUnreachable,

    /// <summary>
    /// If the target is outside of our melee range.
    /// </summary>
    TargetOutOfRange,

    /// <summary>
    /// No dramas.
    /// </summary>
    Normal,
}
