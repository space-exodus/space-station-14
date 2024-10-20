// Exodus-Crawling
using Content.Server.DoAfter;
using Content.Server.NPC.Components;
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
        if (_doAfter.TryStartDoAfter(doAfterArgs, out var doAfterId))
        {
            var standsUp = _entManager.EnsureComponent<NPCStandsUpComponent>(owner);
            standsUp.DoAfter = doAfterId.Value;
        }
    }

    public override HTNOperatorStatus Update(NPCBlackboard blackboard, float frameTime)
    {
        var owner = blackboard.GetValue<EntityUid>(NPCBlackboard.Owner);

        if (_entManager.TryGetComponent<NPCStandsUpComponent>(owner, out var standsUp) && _doAfter.IsRunning(standsUp.DoAfter))
            return HTNOperatorStatus.Continuing;
        return HTNOperatorStatus.Finished;
    }

    public override void TaskShutdown(NPCBlackboard blackboard, HTNOperatorStatus status)
    {
        var owner = blackboard.GetValue<EntityUid>(NPCBlackboard.Owner);
        if (_entManager.HasComponent<NPCStandsUpComponent>(owner))
            _entManager.RemoveComponentDeferred<NPCStandsUpComponent>(owner);
    }
}
