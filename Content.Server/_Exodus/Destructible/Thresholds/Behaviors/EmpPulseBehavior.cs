// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt
using Content.Server.Emp;
using Content.Server.Destructible.Thresholds.Behaviors;
using Content.Server.Destructible;
using Robust.Server.GameObjects;

namespace Content.Server.Exodus.Destructible.Thresholds.Behaviors;

[DataDefinition]
public sealed partial class EmpPulseBehavior : IThresholdBehavior
{
    [DataField("range")]
    public float Range = 1.0f;

    [DataField("energyConsumption")]
    public float EnergyConsumption;

    [DataField("disableDuration")]
    public TimeSpan DisableDuration = TimeSpan.FromSeconds(60);

    public void Execute(EntityUid uid, DestructibleSystem system, EntityUid? cause = null)
    {
        if (!system.EntityManager.TryGetComponent<TransformComponent>(uid, out var xform))
            return;

        if (Range <= 0 || DisableDuration <= TimeSpan.Zero)
            return;

        var emp = system.EntityManager.System<EmpSystem>();
        var transform = system.EntityManager.System<TransformSystem>();
        emp.EmpPulse(transform.GetMapCoordinates(xform), Range, EnergyConsumption, DisableDuration);
    }
}
