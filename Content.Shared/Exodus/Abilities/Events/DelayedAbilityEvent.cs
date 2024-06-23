using Content.Shared.Actions;

namespace Content.Shared.Exodus.Abilities.Events;

public sealed partial class DelayedWorldTargetAbilityEvent : WorldTargetActionEvent, IDelayedAbilityEvent
{
    [DataField]
    public WorldTargetActionEvent? Event = null;

    [DataField]
    public float Delay { get; set; } = 0f;

    public BaseActionEvent? BaseEvent => Event;
}

public sealed partial class DelayedEntityAbilityEvent : EntityTargetActionEvent, IDelayedAbilityEvent
{
    [DataField]
    public EntityTargetActionEvent? Event = null;

    [DataField]
    public float Delay { get; set; } = 0f;

    public BaseActionEvent? BaseEvent => Event;
}

public sealed partial class DelayedInstantAbilityEvent : InstantActionEvent, IDelayedAbilityEvent
{
    [DataField]
    public InstantActionEvent? Event = null;

    [DataField]
    public float Delay { get; set; } = 0f;

    public BaseActionEvent? BaseEvent => Event;
}

public sealed partial class DelayedDirectionalAbilityEvent : DirectionActionEvent, IDelayedAbilityEvent
{
    [DataField]
    public DirectionActionEvent? Event = null;

    [DataField]
    public float Delay { get; set; } = 0f;

    public BaseActionEvent? BaseEvent => Event;
}

public interface IDelayedAbilityEvent
{

    [DataField]
    public float Delay { get; set; }

    [DataField]
    public abstract BaseActionEvent? BaseEvent { get; }

}
