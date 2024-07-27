using Content.Shared.Actions;
using System.Numerics;

namespace Content.Shared.Exodus.Abilities.Events;

[Virtual]
public partial class WorldLineSpawnEvent : WorldTargetActionEvent
{
    [DataField]
    public string SpawnProto = string.Empty;

    [DataField]
    public float LenMultiply = 2.0f;

    [DataField]
    public float StepDelay = 0;

    [DataField]
    public int Step = 1;
}
