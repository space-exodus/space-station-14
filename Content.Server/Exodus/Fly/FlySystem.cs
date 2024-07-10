using System.Numerics;
using Content.Shared.Exodus.Fly;
using Content.Shared.Salvage.Fulton;
using Robust.Shared.Containers;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Random;
using Robust.Server.GameObjects;

namespace Content.Server.Exodus.Fly;

/// <summary>
///
/// </summary>
public sealed class FlySystem : SharedFlySystem
{

    [Dependency] private readonly PhysicsSystem _physics = default!;

    public void TryTakeoff(EntityUid uid, bool force = false, FlyComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        if (CanTakeoff(uid) || force)
            TakeOff(uid, component);
    }

    public void TryLand(EntityUid uid, bool force = false, FlyComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        if (CanLand(uid) || force)
            Land(uid, component);
    }

    private void TakeOff(EntityUid uid, FlyComponent component)
    {
        _physics.SetCanCollide(uid, false);
        Audio.PlayPvs(component.SoundTakeoff, uid);

        RaiseNetworkEvent(new FlyAnimationMessage()
        {
            Entity = GetNetEntity(uid),
            ToAir = true
        });
    }

    private void Land(EntityUid uid, FlyComponent component)
    {
        _physics.SetCanCollide(uid, false);
        Audio.PlayPvs(component.SoundLanding, uid);

        RaiseNetworkEvent(new FlyAnimationMessage()
        {
            Entity = GetNetEntity(uid),
            ToAir = false
        });
    }



}
