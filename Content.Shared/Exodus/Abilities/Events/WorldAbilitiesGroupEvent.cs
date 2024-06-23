using Content.Shared.Actions;

namespace Content.Shared.Exodus.Abilities.Events;

public sealed partial class WorldAbilitiesGroupEvent : WorldTargetActionEvent
{
    [DataField]
    public List<WorldTargetActionEvent> Events = [];
}
