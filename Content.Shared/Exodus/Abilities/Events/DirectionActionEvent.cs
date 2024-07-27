using System.Numerics;
using Content.Shared.Actions;

namespace Content.Shared.Exodus.Abilities.Events;

public abstract partial class DirectionActionEvent : BaseActionEvent
{
    /// <summary>
    ///     The entity that the user targeted.
    /// </summary>
    public Vector2 Direction;
}
