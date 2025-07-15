using Content.Server.Destructible.Thresholds.Behaviors;
using Content.Server.Destructible;
using Content.Shared.Storage;
using Robust.Shared.Serialization;

namespace Content.Server.Exodus.Destructible.Thresholds.Behaviors;

[Serializable]
[DataDefinition]
public sealed partial class DeleteItemsInStorageBehavior : IThresholdBehavior
{
    public void Execute(EntityUid uid, DestructibleSystem system, EntityUid? cause = null)
    {
        if (!system.EntityManager.TryGetComponent<StorageComponent>(uid, out var storageComp))
            return;

        var containedEntities = storageComp.Container.ContainedEntities;

        foreach (var itemUid in containedEntities)
        {
            system.EntityManager.QueueDeleteEntity(itemUid);
        }
    }
}
