// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt
using Robust.Shared.Prototypes;
using Content.Shared.Tag;

namespace Content.Shared.Exodus.LatheSpeedUp.Components;

[RegisterComponent]
public sealed partial class LatheSpeedUpComponent : Component
{
    [DataField]
    public Dictionary<ProtoId<TagPrototype>, float> Modifiers = new();
}
