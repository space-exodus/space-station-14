using Content.Shared.Damage.Systems;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.GameStates;


namespace Content.Shared.Damage.Components;

/// <summary>
/// Reduce stamina damage for entity who has worn it
/// </summary>
[RegisterComponent]
public sealed partial class StaminaArmorComponent : Component
{
    [DataField(required: true)]
    public float Coefficient = 1f;

    /// <summary>
    /// Examine string for stamina resistance.
    /// Passed <c>value</c> from 0 to 100.
    /// </summary>
    [DataField]
    public LocId Examine = "stamina-resistance-coefficient-value";
}
