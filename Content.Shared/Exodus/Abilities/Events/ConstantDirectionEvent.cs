using System.Numerics;
using Content.Shared.Actions;

namespace Content.Shared.Exodus.Abilities.Events;

public sealed partial class ConstantDirectionEvent : InstantActionEvent
{
    [DataField]
    public DirectionActionEvent? Event = null;

    [DataField]
    public Vector2 ConstDirection = Vector2.Zero;
}
