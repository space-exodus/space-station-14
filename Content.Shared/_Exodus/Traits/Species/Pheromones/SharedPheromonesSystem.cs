// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

namespace Content.Shared.Exodus.Traits.Species.Pheromones;

public abstract partial class SharedPheromonesSystem : EntitySystem
{
    public bool CanSeePheromones(EntityUid? entity)
    {
        if (entity == null)
            return false;

        return HasComp<PheromonesCommunicationComponent>(entity.Value);
    }
}
