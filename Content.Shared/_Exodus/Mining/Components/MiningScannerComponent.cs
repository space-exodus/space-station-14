// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt
// Based on https://github.com/space-wizards/space-station-14/tree/f9320aacd72315a38ddb9a4d73dbd6231a8c1db4/Content.Shared/Mining/Components/MiningScannerComponent.cs

using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared.Exodus.Mining.Components;

/// <summary>
/// This is a component that, when held in the inventory or pocket of a player, gives the the MiningOverlay.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(SharedMiningScannerSystem)), AutoGenerateComponentState, AutoGenerateComponentPause]
public sealed partial class MiningScannerComponent : Component
{
    [DataField]
    public float Range = 5;

    [DataField, AutoNetworkedField]
    public TimeSpan PingDelay = TimeSpan.FromSeconds(5);

    [DataField, AutoNetworkedField, AutoPausedField]
    public TimeSpan NextPingTime = TimeSpan.MaxValue;

    [DataField, AutoNetworkedField]
    public float AnimationDuration = 1.5f;

    [DataField, AutoNetworkedField]
    public SoundSpecifier? PingSound = new SoundPathSpecifier("/Audio/Machines/sonar-ping.ogg")
    {
        Params = new AudioParams
        {
            Volume = -3,
        }
    };
}
