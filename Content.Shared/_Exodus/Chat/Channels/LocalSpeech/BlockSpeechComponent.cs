// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Robust.Shared.GameStates;

namespace Content.Shared.Exodus.Chat.Channels.LocalSpeech;

/// <summary>
/// Assigned to entities which can block speech hearing,
/// it is counted only if it is anchored
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class BlockSpeechComponent : Component
{
    [DataField, AutoNetworkedField]
    public float Resistance = 3f;

    /// <summary>
    /// Ignored resistance and just fully blocks speech
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool FullBlock = false;
}
