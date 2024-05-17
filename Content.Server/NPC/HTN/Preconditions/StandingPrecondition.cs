// Exodus-Crawling
using Content.Shared.Standing;

namespace Content.Server.NPC.HTN.Preconditions;

/// <summary>
/// Checks if the owner is standing or not
/// </summary>
public sealed partial class StandingPrecondition : HTNPrecondition
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    [DataField("isStanding"), ViewVariables(VVAccess.ReadWrite)]
    public bool IsStanding = true;

    public override bool IsMet(NPCBlackboard blackboard)
    {
        var owner = blackboard.GetValue<EntityUid>(NPCBlackboard.Owner);

        if (!_entityManager.TryGetComponent<StandingStateComponent>(owner, out var standing))
            return true;

        return IsStanding && standing.Standing || !IsStanding && !standing.Standing;
    }
}
