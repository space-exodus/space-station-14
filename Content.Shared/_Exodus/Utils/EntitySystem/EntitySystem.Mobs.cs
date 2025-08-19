// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.Interaction.Events;

namespace Content.Shared.Exodus.Utils;

public static class EntitySystemMobsExt
{
    public static bool IsInConscious(this EntitySystem sys, EntityUid? mob, IEntityManager entity)
    {
        if (mob is not { } mobUid)
            return false;

        var ev = new ConsciousAttemptEvent(mobUid);
        entity.EventBus.RaiseLocalEvent(mobUid, ref ev, false);

        return !ev.Cancelled;
    }
}
