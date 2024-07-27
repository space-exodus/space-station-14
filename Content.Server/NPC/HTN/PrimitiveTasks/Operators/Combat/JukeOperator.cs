using Content.Server.NPC.Components;

namespace Content.Server.NPC.HTN.PrimitiveTasks.Operators.Combat;

public sealed partial class JukeOperator : HTNOperator, IHtnConditionalShutdown
{
    [Dependency] private readonly IEntityManager _entManager = default!;

    [DataField("jukeType")]
    public JukeType JukeType = JukeType.AdjacentTile;

    // Exodus-Lavaland-AdvancedAI-Start
    [DataField("jukeDuration")]
    public float JukeDuration = 0.5f;
    // Exodus-Lavaland-AdvancedAI-End

    [DataField("shutdownState")]
    public HTNPlanState ShutdownState { get; private set; } = HTNPlanState.PlanFinished;

    public override void Startup(NPCBlackboard blackboard)
    {
        base.Startup(blackboard);
        var juke = _entManager.EnsureComponent<NPCJukeComponent>(blackboard.GetValue<EntityUid>(NPCBlackboard.Owner));
        juke.JukeType = JukeType;
        juke.JukeDuration = JukeDuration; // Exodus-Lavaland-AdvancedAI
    }

    public override HTNOperatorStatus Update(NPCBlackboard blackboard, float frameTime)
    {
        return HTNOperatorStatus.Finished;
    }

    public void ConditionalShutdown(NPCBlackboard blackboard)
    {
        _entManager.RemoveComponent<NPCJukeComponent>(blackboard.GetValue<EntityUid>(NPCBlackboard.Owner));
    }
}
