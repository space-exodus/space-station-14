// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.Exodus.Stealth.Components;

namespace Content.Shared.Exodus.Stealth;

public sealed partial class StealthOnSpawnSystem : EntitySystem
{
    [Dependency] private readonly SharedStealthSystem _stealthSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StealthOnSpawnComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<StealthOnSpawnComponent, ComponentShutdown>(OnShutdown);
    }

    private void OnInit(EntityUid uid, StealthOnSpawnComponent comp, ComponentInit args)
    {
        if (Paused(uid))
            return;

        if (!_stealthSystem.RequestStealth(uid, uid, comp.Stealth))
            return;
    }

    private void OnShutdown(EntityUid uid, StealthOnSpawnComponent comp, ComponentShutdown args)
    {
        if (!_stealthSystem.RemoveRequest(uid, uid))
            return;
    }
}
