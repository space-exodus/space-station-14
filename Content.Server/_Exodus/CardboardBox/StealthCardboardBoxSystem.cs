// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.CardboardBox.Components;
using Content.Shared.Exodus.CardboardBox.Components;
using Content.Shared.Exodus.Stealth.Components;
using Content.Shared.Exodus.Stealth;
using Content.Shared.Storage.Components;
using Content.Server.Storage.Components;

namespace Content.Server.Exodus.CardboardBox;

public sealed class StealthCardboardBoxSystem : EntitySystem
{
    [Dependency] private readonly InstantStealthSystem _instantStealth = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CardboardBoxComponent, StorageAfterOpenEvent>(AfterStorageOpen);
        SubscribeLocalEvent<CardboardBoxComponent, StorageAfterCloseEvent>(AfterStorageClosed);
    }

    private void AfterStorageOpen(EntityUid uid, CardboardBoxComponent component, ref StorageAfterOpenEvent args)
    {
        if (!HasComp<StealthCardboardBoxComponent>(uid))
            return;

        _instantStealth.SetEnabled(uid, false);
    }

    private void AfterStorageClosed(EntityUid uid, CardboardBoxComponent component, ref StorageAfterCloseEvent args)
    {
        if (!HasComp<StealthCardboardBoxComponent>(uid))
            return;

        _instantStealth.SetEnabled(uid, true);
    }
}

