using Content.Shared.Actions;
using System.Numerics;

namespace Content.Shared.Exodus.Abilities.Events;

public sealed partial class WorldMultishootEvent : WorldTargetActionEvent
{
    [DataField]
    public int ShootCnt = 3;

    [DataField]
    public float Angle = (float) Math.PI / 4;

    [DataField]
    public bool Clockwise = true;

    [DataField]
    public float ShootDelay = 0;

    [DataField]
    public DirectionActionEvent? Event = null;
}
