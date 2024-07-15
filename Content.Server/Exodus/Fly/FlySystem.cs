using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.Exodus.Fly;
using Robust.Shared.Console;
using Robust.Shared.Physics.Systems;

namespace Content.Server.Exodus.Fly;

/// <summary>
///
/// </summary>
public sealed class FlySystem : SharedFlySystem
{
    [Dependency] private readonly IConsoleHost _console = default!;

    public override void Initialize()
    {
        base.Initialize();

        _console.RegisterCommand("landentity", LandEntityCommand);
        _console.RegisterCommand("takeoffentity", TakeoffEntityCommand);
    }


    [AdminCommand(AdminFlags.Fun)]
    private void LandEntityCommand(IConsoleShell shell, string argstr, string[] args)
    {
        if (args.Length != 1)
        {
            shell.WriteError(Loc.GetString("cmd-landentity-invalid"));
            return;
        }

        if (!NetEntity.TryParse(args[0], out var uidNet) || !TryGetEntity(uidNet, out var uid))
        {
            shell.WriteLine($"No entity found with netUid {uidNet}");
            return;
        }

        TryLand(uid.Value);
    }

    [AdminCommand(AdminFlags.Fun)]
    private void TakeoffEntityCommand(IConsoleShell shell, string argstr, string[] args)
    {
        if (args.Length != 1)
        {
            shell.WriteError(Loc.GetString("cmd-takeoffentity-invalid"));
            return;
        }

        if (!NetEntity.TryParse(args[0], out var uidNet) || !TryGetEntity(uidNet, out var uid))
        {
            shell.WriteLine($"No entity found with netUid {uidNet}");
            return;
        }

        TryTakeoff(uid.Value);
    }


    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<FlyComponent>();
        while (query.MoveNext(out var uid, out var flyComp))
        {
            if (flyComp.DoAnimation &&
                flyComp.AnimationTimeEnd <= Timing.CurTime)
            {
                if (flyComp.IsInAir)
                {
                    RaiseNetworkEvent(new LandMessage()
                    {
                        Entity = GetNetEntity(uid)
                    });

                    flyComp.IsInAir = false;
                }
                else
                {
                    SetCollidable(uid, false);

                    RaiseNetworkEvent(new TakeoffMessage()
                    {
                        Entity = GetNetEntity(uid)
                    });

                    flyComp.IsInAir = true;
                }

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
        Audio.PlayPvs(component.SoundLand, uid);

        component.DoAnimation = true;
        component.AnimationTimeEnd = Timing.CurTime + TimeSpan.FromSeconds(component.LandTime);

        SetCollidable(uid, true);

        RaiseNetworkEvent(new LandAnimationMessage()
        {
            Entity = GetNetEntity(uid)
        });
    }



}
