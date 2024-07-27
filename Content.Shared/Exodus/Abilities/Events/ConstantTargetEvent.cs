using System.Numerics;
using Content.Shared.Actions;

namespace Content.Shared.Exodus.Abilities.Events;

public sealed partial class ConstantWorldEvent : InstantActionEvent
{
    [DataField]
    public WorldTargetActionEvent? Event = null;

    [DataField]
    public Vector2 ConstOffset = Vector2.Zero;
}
