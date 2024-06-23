using Content.Shared.Actions;

namespace Content.Shared.Exodus.Abilities.Events;

public sealed partial class EntityToCoordEvent : EntityTargetActionEvent
{
    [DataField]
    public WorldTargetActionEvent? Event = null;
}
