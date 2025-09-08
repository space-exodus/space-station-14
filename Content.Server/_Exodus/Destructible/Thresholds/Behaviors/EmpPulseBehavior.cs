// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt
using Content.Server.Emp;
using Robust.Shared.Map;
using Content.Server.Destructible.Thresholds.Behaviors;
using Content.Server.Destructible;

namespace Content.Server.Exodus.Destructible.Thresholds.Behaviors;

[DataDefinition]
public sealed partial class EmpPulseBehavior : IThresholdBehavior
{
    [DataField("range")]
    public float Range = 1.0f;

    [DataField("energyConsumption")]
    public float EnergyConsumption;

    [DataField("disableDuration")]
    public float DisableDuration = 60f;

    public void Execute(EntityUid uid, DestructibleSystem system, EntityUid? cause = null)
    {
        if (!system.EntityManager.TryGetComponent<TransformComponent>(uid, out var xform))
            return;

        if (Range <= 0 || DisableDuration <= 0)
            return;

        var emp = system.EntityManager.System<EmpSystem>();
        emp.EmpPulse(xform.Coordinates, Range, EnergyConsumption, DisableDuration);
    }
}
