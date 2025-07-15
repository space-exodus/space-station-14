using Robust.Shared.Prototypes;
using Content.Shared.Tag;

namespace Content.Shared.Exodus.LatheSpeedUp.Components;

[RegisterComponent]
public sealed partial class LatheSpeedUpComponent : Component
{
    [DataField]
    public Dictionary<ProtoId<TagPrototype>, float> Modifiers = new();

    public const string ID = "latheSpeedUp";
}
