// using Content.Server.Body.Components;
using System.Linq; // Exodus-Species
using Content.Shared.Body.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array; // Exodus-Species

namespace Content.Shared.EntityEffects.EffectConditions;

/// <summary>
///     Requires that the metabolizing organ is or is not tagged with a certain MetabolizerType
/// </summary>
public sealed partial class OrganType : EventEntityEffectCondition<OrganType>
{
    // Exodus-Species-Start
    /// <summary>
    /// Checks for any overlap with metabolizer, if you need for metabolizer to have exactly two types - just create another one condition
    /// </summary>
    [DataField(required: true, customTypeSerializer: typeof(PrototypeIdArraySerializer<MetabolizerTypePrototype>))]
    public string[] Type = default!;
    // Exodus-Species-End

    /// <summary>
    ///     Does this condition pass when the organ has the type, or when it doesn't have the type?
    /// </summary>
    [DataField]
    public bool ShouldHave = true;

    public override string GuidebookExplanation(IPrototypeManager prototype)
    {
        // Exodus-Species-Start | Ugly but I don't want to make refactor of guidebook explanation generation
        return string.Join(", ", Type.Select(type => Loc.GetString("reagent-effect-condition-guidebook-organ-type",
            ("name", prototype.Index<MetabolizerTypePrototype>(type).LocalizedName),
            ("shouldhave", ShouldHave))));
        // Exodus-Species-End
    }
}
