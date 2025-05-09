// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.EntityEffects;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Server.Exodus.EntityEffects.Effects;

[UsedImplicitly]
public sealed partial class DeleteEntityReaction : EntityEffect
{
    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
        => Loc.GetString("reagent-effect-guidebook-delete-entity-reaction");

    public override void Effect(EntityEffectBaseArgs args)
    {
        args.EntityManager.QueueDeleteEntity(args.TargetEntity);
    }
}
