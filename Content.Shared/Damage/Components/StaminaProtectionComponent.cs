// Exodus - Stamina Rework
// Use this comp and StaminaArmor instead of StaminaResistance
using Robust.Shared.GameStates;


namespace Content.Shared.Damage.Components;

/// <summary>
/// Component that provides entities with stamina resistance.
/// </summary>
/// <remarks>
/// This is desirable over just using damage modifier sets, given that equipment like bomb-suits need to
/// significantly reduce the damage, but shouldn't be silly overpowered in regular combat.
/// </remarks>
[NetworkedComponent, RegisterComponent, AutoGenerateComponentState]
public sealed partial class StaminaProtectionComponent : Component
{
    /// <summary>
    /// The stamina resistance coefficient, This fraction is multiplied into the total resistance.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float Coefficient = 1;
}
