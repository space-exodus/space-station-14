// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared.Exodus.Mining.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, AutoGenerateComponentPause, Access(typeof(SharedMiningScannerSystem))]
public sealed partial class MiningScannerUserComponent : Component
{
    [DataField]
    public bool QueueRemoval = false;

    [DataField, AutoNetworkedField]
    public TimeSpan PingDelay = TimeSpan.FromSeconds(3.5f);

    [DataField, AutoNetworkedField, AutoPausedField]
    public TimeSpan NextPingTime = TimeSpan.MaxValue;

    [DataField, AutoNetworkedField]
    public float AnimationDuration = 1.5f;

    [DataField, AutoNetworkedField]
    public float ViewRange = 5f;

    [DataField, AutoNetworkedField]
    public SoundSpecifier? PingSound = new SoundPathSpecifier("/Audio/Machines/sonar-ping.ogg")
    {
        Params = new AudioParams
        {
            Volume = -3,
        }
    };
}
