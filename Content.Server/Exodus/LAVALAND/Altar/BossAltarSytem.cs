using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Console;
using Robust.Shared.Physics.Systems;

namespace Content.Server.Exodus.Lavaland.Altar;

public sealed partial class BossAltarSystem : EntitySystem
{
    [Dependency] private readonly IConsoleHost _console = default!;
    [Dependency] private readonly SharedAudioSystem _audioSystem = default!;
    [Dependency] private readonly TransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();

        _console.RegisterCommand("bossaltaractivate", BossAltarActivateCommand);
    }


    public void Activate(EntityUid uid)
    {
        if (!TryComp<BossAltarComponent>(uid, out var comp))
            return;

        var spawn = Spawn(comp.BossSpawnProto, Transform(uid).Coordinates);
        _audioSystem.PlayPvs(comp.ActivateSound, spawn, new AudioParams() { MaxDistance = 30 });

        if (comp.DespawnAfterActivate)
            QueueDel(uid);
    }


    [AdminCommand(AdminFlags.Fun)]
    private void BossAltarActivateCommand(IConsoleShell shell, string argstr, string[] args)
    {
        if (args.Length != 1)
        {
            shell.WriteError(Loc.GetString("cmd-bossaltar-invalid"));
            return;
        }

        if (!NetEntity.TryParse(args[0], out var uidNet) || !TryGetEntity(uidNet, out var uid))
        {
            shell.WriteLine($"No entity found with netUid {uidNet}");
            return;
        }

        Activate(uid.Value);
    }
}
