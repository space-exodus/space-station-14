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
    [Dependency] private readonly SharedStealthSystem _stealth = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CardboardBoxComponent, ComponentInit>(OnStartup);
        SubscribeLocalEvent<CardboardBoxComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<CardboardBoxComponent, StorageAfterOpenEvent>(AfterStorageOpen);
        SubscribeLocalEvent<CardboardBoxComponent, StorageAfterCloseEvent>(AfterStorageClosed);
    }

    private void OnStartup(EntityUid uid, CardboardBoxComponent component, ComponentInit args)
    {
        if (!TryComp<StealthCardboardBoxComponent>(uid, out var stealthBox))
            return;

        if (TryComp<EntityStorageComponent>(uid, out var storage) && storage.Open)
            return;

        _stealth.RequestStealth(uid, nameof(StealthCardboardBoxSystem), stealthBox.Stealth);
    }

    private void OnShutdown(EntityUid uid, CardboardBoxComponent component, ComponentShutdown args)
    {
        if (!TryComp<StealthCardboardBoxComponent>(uid, out var stealthBox))
            return;

        _stealth.RemoveRequest(nameof(StealthCardboardBoxSystem), uid);
    }

    private void AfterStorageOpen(EntityUid uid, CardboardBoxComponent component, ref StorageAfterOpenEvent args)
    {
        if (!TryComp<StealthCardboardBoxComponent>(uid, out var stealthBox))
            return;

        _stealth.RemoveRequest(nameof(StealthCardboardBoxSystem), uid);
    }

    private void AfterStorageClosed(EntityUid uid, CardboardBoxComponent component, ref StorageAfterCloseEvent args)
    {
        if (!TryComp<StealthCardboardBoxComponent>(uid, out var stealthBox))
            return;

        _stealth.RequestStealth(uid, nameof(StealthCardboardBoxSystem), stealthBox.Stealth);
    }
}

