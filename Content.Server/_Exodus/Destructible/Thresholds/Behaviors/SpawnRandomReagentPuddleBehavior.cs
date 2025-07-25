// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt
using Content.Server.Destructible.Thresholds.Behaviors;
using Content.Server.Destructible;
using Content.Server.Fluids.EntitySystems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using System.Linq;

namespace Content.Server.Exodus.Destructible.Thresholds.Behaviors;

[Serializable]
[DataDefinition]
public sealed partial class SpawnReagentPuddleBehavior : IThresholdBehavior
{
    [DataField("possibleReagents")]
    public Dictionary<string, FixedPoint2> PossibleReagents = new();

    public void Execute(EntityUid uid, DestructibleSystem system, EntityUid? cause = null)
    {
        if (PossibleReagents.Count == 0)
            return;

        var random = IoCManager.Resolve<IRobustRandom>();
        var reagentId = random.Pick(PossibleReagents.Keys.ToList());
        var quantity = PossibleReagents[reagentId];

        var solution = new Solution();
        solution.AddReagent(reagentId, quantity);

        var puddleSystem = system.EntityManager.System<PuddleSystem>();
        puddleSystem.TrySpillAt(uid, solution, out _);
    }
}
