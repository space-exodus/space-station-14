using System.Numerics;
using Content.Shared.Exodus.Fly;
using Content.Shared.Salvage.Fulton;
using Robust.Shared.Containers;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Random;
using Robust.Shared.Physics.System;
using Robust.Server.GameObjects;

namespace Content.Server.Exodus.Fly;

/// <summary>
///
/// </summary>
public sealed class FlySystem : SharedFlySystem
{

    [Dependency] private readonly SharedPhysicsSystem _physics = default!;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<FlyComponent>();
        while (query.MoveNext(out var uid, out var flyComp))
        {
            if (flyComp.DoAnimation &&
                flyComp.AnimationTimeEnd >= Timing.CurTime)
            {
                if (flyComp.IsInAir)
                {
                    _physics.SetCanCollide(uid, true);

                    RaiseNetworkEvent(new LandMessage()
                    {
                        Entity = GetNetEntity(uid)
                    });
                }
                else
                {
                    _physics.SetCanCollide(uid, false);

                    RaiseNetworkEvent(new TakeoffMessage()
                    {
                        Entity = GetNetEntity(uid)
                    });
                }

                flyComp.IsInAir = !flyComp.IsInAir;
                flyComp.DoAnimation = false;
            }

        }
    }

    public void TryTakeoff(EntityUid uid, FlyComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        if (CanTakeoff(uid))
            TakeOff(uid, component);
    }

    public void TryLand(EntityUid uid, FlyComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        if (CanLand(uid))
            Land(uid, component);
    }

    private void TakeOff(EntityUid uid, FlyComponent component)
    {
        Audio.PlayPvs(component.SoundTakeoff, uid);

        component.DoAnimation = true;
        component.AnimationTimeEnd = Timing.CurTime + TimeSpan.FromSeconds(component.TakeoffTime);

        RaiseNetworkEvent(new TakeoffAnimationMessage()
        {
            Entity = GetNetEntity(uid)
        });
    }

    private void Land(EntityUid uid, FlyComponent component)
    {
        Audio.PlayPvs(component.SoundLanding, uid);

        component.DoAnimation = true;
        component.AnimationTimeEnd = Timing.CurTime + TimeSpan.FromSeconds(component.LandTime);

        RaiseNetworkEvent(new LandAnimationMessage()
        {
            Entity = GetNetEntity(uid)
        });
    }



}
