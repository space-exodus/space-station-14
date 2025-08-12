// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt


using Content.Shared.Exodus.Stealth.Components;

namespace Content.Shared.Exodus.CardboardBox.Components;

[RegisterComponent]
public sealed partial class StealthCardboardBoxComponent : Component
{
    [DataField("stealth")]
    public StealthData Stealth = new();
}
