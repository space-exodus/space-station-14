using Content.Shared.Actions;

namespace Content.Shared.Exodus.Abilities.Events;

public sealed partial class EntityAbilitiesGroupEvent : EntityTargetActionEvent
{
    [DataField]
    public List<EntityTargetActionEvent> Events = [];
}
