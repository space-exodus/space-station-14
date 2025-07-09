// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.Actions;
using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Exodus.Traits.Species.Pheromones;

[RegisterComponent, NetworkedComponent]
public sealed partial class PheromonesCommunicationComponent : Component
{
    [DataField]
    public EntProtoId ActionPrototype = "ActionPheromones";

    [DataField]
    public EntityUid? ActionEntity;

    [DataField]
    public FixedPoint2 Range = 1.5f;

    [DataField]
    public Color Color = Color.Yellow;

    /// <summary>
    /// Entity created when kidan is tries to create pheromones message without any entity
    /// </summary>
    [DataField]
    public EntProtoId PheromonesCloud = "PheromonesEffect";
}

public sealed partial class TryMarkWithPheromonesEvent : WorldTargetActionEvent
{
}
