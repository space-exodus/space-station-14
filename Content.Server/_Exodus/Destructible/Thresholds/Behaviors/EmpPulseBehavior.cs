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
        var emp = system.EntityManager.System<EmpSystem>();
        var xform = system.EntityManager.GetComponent<TransformComponent>(uid);
        emp.EmpPulse(xform.Coordinates, Range, EnergyConsumption, DisableDuration);
    }
}
