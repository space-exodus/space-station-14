using Content.Shared.Movement.Pulling.Components;
using Content.Shared.NPC.Systems;
using Content.Shared.Pulling;
using PullingSystem = Content.Shared.Movement.Pulling.Systems.PullingSystem;

namespace Content.Server.NPC.HTN.Preconditions;

/// <summary>
/// Checks if the owner is being pulled or not.
/// </summary>
public sealed partial class PulledPrecondition : HTNPrecondition
{
    [Dependency] private readonly IEntityManager _entityManager = default!; // Exodus-MRP NPC
    private NpcFactionSystem _npcFaction = default!; // Exodus-MRP NPC
    private PullingSystem _pulling = default!;

    [ViewVariables(VVAccess.ReadWrite)] [DataField("isPulled")] public bool IsPulled = true;
    [ViewVariables(VVAccess.ReadWrite)] [DataField("isHostilePulling")] public bool IsHostilePulling = true; // Exodus-MRP NPC

    public override void Initialize(IEntitySystemManager sysManager)
    {
        base.Initialize(sysManager);
        _pulling = sysManager.GetEntitySystem<PullingSystem>();
        _npcFaction = sysManager.GetEntitySystem<NpcFactionSystem>(); // Exodus-MRP NPC
    }

    public override bool IsMet(NPCBlackboard blackboard)
    {
        var owner = blackboard.GetValue<EntityUid>(NPCBlackboard.Owner);

        // Exodus-MRP NPC-Start
        if (!_entityManager.TryGetComponent<PullableComponent>(owner, out var pullable))
            return false;

        return !IsPulled && !_pulling.IsPulled(owner) ||
                IsPulled && _pulling.IsPulled(owner) &&

                pullable.Puller is not null &&
                    (IsHostilePulling && _npcFaction.IsEntityHostile(owner, pullable.Puller.Value) ||
                    !IsHostilePulling && !_npcFaction.IsEntityHostile(owner, pullable.Puller.Value));
        // Exodus-MRP NPC-End
    }
}
