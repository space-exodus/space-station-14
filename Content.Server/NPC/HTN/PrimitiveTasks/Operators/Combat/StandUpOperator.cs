// Exodus-Crawling
using Content.Server.DoAfter;
using Content.Shared.DoAfter;
using Content.Shared.Standing;

namespace Content.Server.NPC.HTN.PrimitiveTasks.Operators.Combat;

public sealed partial class StandUpOperator : HTNOperator
{
    [Dependency] private readonly IEntityManager _entManager = default!;
    private DoAfterSystem _doAfter = default!;

    [DataField("shutdownState")]
    public HTNPlanState ShutdownState { get; private set; } = HTNPlanState.TaskFinished;

    public override void Initialize(IEntitySystemManager sysManager)
    {
        base.Initialize(sysManager);
        _doAfter = sysManager.GetEntitySystem<DoAfterSystem>();
    }

    public override void Startup(NPCBlackboard blackboard)
    {
        base.Startup(blackboard);
        var owner = blackboard.GetValue<EntityUid>(NPCBlackboard.Owner);

        if (!_entManager.TryGetComponent<StandingStateComponent>(owner, out var standing) || standing.Standing)
            return;

        var doAfterArgs = new DoAfterArgs(_entManager, owner, standing.StandDelay, new StandDoAfterEvent(), owner)
        {
            CancelDuplicate = true,
            BreakOnDamage = true,
        };
        _doAfter.TryStartDoAfter(doAfterArgs);
    }

    public override HTNOperatorStatus Update(NPCBlackboard blackboard, float frameTime)
    {
        return HTNOperatorStatus.Finished;
    }
}
