// Exodus
using Robust.Shared.GameStates;
using Robust.Shared.Noise;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared.Damage.Components;

/// <summary>
/// Add to an entity to paralyze it whenever it reaches critical amounts of Stamina DamageType.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState] // Exodus - Stamina Refactor | Remove AutoGenerateComponentPause
public sealed partial class StaminaComponent : Component
{
    // Exodus - Stamina Refactor | Remove Critical State

    /// <summary>
    /// How much stamina reduces per second.
    /// </summary>
    [DataField, AutoNetworkedField] // Exodus - Stamina Refactor | Remove VV
    public float Decay = 3f;

    /// <summary>
    /// How much time after receiving damage until stamina starts decreasing.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public TimeSpan Cooldown = TimeSpan.FromSeconds(1f); // Exodus - Stamina Refactor | Change to TimeSpan, decrease duration

    /// <summary>
    /// How much stamina damage this entity has taken.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public float StaminaDamage;

    /// <summary>
    /// How much stamina damage is required to entire stam crit.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public float CritThreshold = 100f;

    /// <summary>
    /// How long will this mob be stunned for?
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public TimeSpan StunTime = TimeSpan.FromSeconds(2); // Exodus - Balance

    // Exodus - Stamina refactor - start

    /// <summary>
    /// Save time of end of last stamina stun
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public TimeSpan StunEnd = TimeSpan.Zero;

    /// <summary>
    /// Save time of last stamina stun
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public TimeSpan LastDamage = TimeSpan.Zero;

    public TimeSpan RestoreDamageStart => LastDamage + Cooldown;

    /// <summary>
    /// Modify stamina damage when walking
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField]
    public float DamageWalkingModify = 0.88f;
    // Exodus - end

    // Exodus - Stamina Refactor | Remove Next Update
    // Exodus - Stamina Refactor | Remove Stamina Alert
    // Exodus - Stamina Refactor | Remove stun threshoulds

    // Exodus - Stamina Rework


    /// <summary>
    /// Is this entity in danger?
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly), DataField, AutoNetworkedField]
    public bool IsInDanger = false;



    // There's RS-trigger schema
    // It's nessesary 'cause some systems use its for cast decay

    /// <summary>
    /// Some systems decays depends on stamina damage
    /// </summary>
    public float DangerThreshold => IsInDanger
        ? MathF.Min(CritThreshold * 0.89f, CritThreshold - 2f)
        : CritThreshold * 0.90f;

    public void UpdateIsInDanger()
    {
        IsInDanger = StaminaDamage >= DangerThreshold;
    }

    /// <summary>
    /// How much stamina reduces per second.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public float BaseDecay = 10.0f;
    // Exodus - End
}
