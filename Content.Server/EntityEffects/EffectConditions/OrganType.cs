using System.Linq; // Exodus-Species
using Content.Server.Body.Components;
using Content.Shared.Body.Prototypes;
using Content.Shared.EntityEffects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array; // Exodus-Species

namespace Content.Server.EntityEffects.EffectConditions;

/// <summary>
///     Requires that the metabolizing organ is or is not tagged with a certain MetabolizerType
/// </summary>
public sealed partial class OrganType : EntityEffectCondition
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

    public override bool Condition(EntityEffectBaseArgs args)
    {
        if (args is EntityEffectReagentArgs reagentArgs)
        {
            if (reagentArgs.OrganEntity == null)
                return false;

            return Condition(reagentArgs.OrganEntity.Value, reagentArgs.EntityManager);
        }

        // TODO: Someone needs to figure out how to do this for non-reagent effects.
        throw new NotImplementedException();
    }

    public bool Condition(Entity<MetabolizerComponent?> metabolizer, IEntityManager entMan)
    {
        metabolizer.Comp ??= entMan.GetComponentOrNull<MetabolizerComponent>(metabolizer.Owner);
        // Exodus-Species-Start | It's ugly but I have no other way, I just can't specify the valid type due to RobustToolbox limitations
        if (metabolizer.Comp != null && metabolizer.Comp.MetabolizerTypes != null)
        {
            var metabolizerTypeIds = metabolizer.Comp.MetabolizerTypes.Select(id => id.Id);
            if (Type.Any(type => metabolizerTypeIds.Contains(type)))
                return ShouldHave;
        }
        // Exodus-Species-End
        return !ShouldHave;
    }

    public override string GuidebookExplanation(IPrototypeManager prototype)
    {
        // Exodus-Species-Start | Ugly but I don't want to make refactor of guidebook explanation generation
        return string.Join(", ", Type.Select(type => Loc.GetString("reagent-effect-condition-guidebook-organ-type",
            ("name", prototype.Index<MetabolizerTypePrototype>(type).LocalizedName),
            ("shouldhave", ShouldHave))));
        // Exodus-Species-End
    }
}
