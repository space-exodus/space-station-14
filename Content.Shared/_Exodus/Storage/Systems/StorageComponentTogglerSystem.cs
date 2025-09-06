// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.Exodus.Storage.Components;
using Content.Shared.Storage.Components;

namespace Content.Shared.Exodus.Storage;

public sealed class StorageComponentTogglerSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StorageComponentTogglerComponent, StorageBeforeOpenEvent>(OnOpen);
        SubscribeLocalEvent<StorageComponentTogglerComponent, StorageBeforeCloseEvent>(OnClose);
    }

    private void OnOpen(EntityUid uid, StorageComponentTogglerComponent comp, StorageBeforeOpenEvent args)
    {
        if (TerminatingOrDeleted(uid))
            return;

        EntityManager.RemoveComponents(uid, comp.RemoveComponents ?? comp.Components);
    }

    private void OnClose(EntityUid uid, StorageComponentTogglerComponent comp, StorageBeforeCloseEvent args)
    {
        if (TerminatingOrDeleted(uid))
            return;

        EntityManager.AddComponents(uid, comp.Components);
    }
}
