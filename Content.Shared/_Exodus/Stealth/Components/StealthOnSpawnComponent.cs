// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.Exodus.Stealth;

namespace Content.Shared.Exodus.Stealth.Components;

[RegisterComponent]
public sealed partial class StealthOnSpawnComponent : Component
{
    [DataField]
    public StealthData Stealth = new();
}