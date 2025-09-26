// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt
using Content.Shared.Exodus.LatheSpeedUp.Components;
using Content.Shared.Tag;
using Content.Shared.Containers.ItemSlots;

namespace Content.Shared.Exodus.LatheSpeedUp;

public sealed class LatheSpeedUpSystem : EntitySystem
{
    [Dependency] private readonly ItemSlotsSystem _itemSlotsSystem = default!;
    [Dependency] private readonly TagSystem _tagSystem = default!;

    public TimeSpan ApplySpeed(Entity<LatheSpeedUpComponent?> ent, TimeSpan time)
    {
        if (!Resolve(ent, ref ent.Comp, false))
            return time;

        if (!TryComp<ItemSlotsComponent>(ent.Owner, out var itemSlotsComp))
            return time;

        foreach (var (slotId, slot) in itemSlotsComp.Slots)
        {
            if (slot.Item is not { } itemUid)
                continue;

            foreach (var (tag, modifier) in ent.Comp.Modifiers)
            {
                if (_tagSystem.HasTag(itemUid, tag))
                {
                    time *= modifier;
                }
            }
        }

        return time;

    }
}
