// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared.Exodus.Stealth.Components;


[RegisterComponent, NetworkedComponent]
public sealed partial class StealthOnHeldComponent : Component
{
    [DataField]
    public bool StealthOnHeldEnabled = true;
    
    [DataField("enabled")]
    public bool Enabled = true;

    [DataField("enabledOnDeath")]
    public bool EnabledOnDeath = true;

    [DataField("hadOutline")]
    public bool HadOutline;

    [DataField("examineThreshold")]
    public float ExamineThreshold = 0.5f;

    [DataField("lastVisibility")]
    public float LastVisibility = 1;

    [DataField("lastUpdate", customTypeSerializer:typeof(TimeOffsetSerializer))]
    public TimeSpan? LastUpdated;

    [DataField("minVisibility")]
    public float MinVisibility = -1f;

    [DataField("maxVisibility")]
    public float MaxVisibility = 1.5f;

    [DataField("examinedDesc")]
    public string ExaminedDesc = "stealth-visual-effect";

    //StealthOnMoveComponent
    [DataField("stealthOnMoveEnabled")]
    public bool StealthOnMoveEnabled = false;

    [DataField("passiveVisibilityRate")]
    public float PassiveVisibilityRate = -0.15f;

    [DataField("movementVisibilityRate")]
    public float MovementVisibilityRate = 0.2f;
}
