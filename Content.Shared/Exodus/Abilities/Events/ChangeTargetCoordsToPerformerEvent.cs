using Content.Shared.Actions;

namespace Content.Shared.Exodus.Abilities.Events;

public sealed partial class ChangeTargetCoordsToPerformerEvent : WorldTargetActionEvent
{
    [DataField]
    public WorldTargetActionEvent? Event;
}
