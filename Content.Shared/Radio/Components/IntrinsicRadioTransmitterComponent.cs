using Content.Shared.Chat;
using Content.Shared.Chat.Prototypes; // Exodus-Kidans
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Radio.Components;

/// <summary>
///     This component allows an entity to directly translate spoken text into radio messages (effectively an intrinsic
///     radio headset).
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class IntrinsicRadioTransmitterComponent : Component
{
    [DataField]
    public HashSet<ProtoId<RadioChannelPrototype>> Channels = new() { SharedChatSystem.CommonChannel };

    // Exodus-Kidans-Start
    /// <summary>
    /// Emote sent by entity when it speaks. Does nothing when nothing specified.
    /// </summary>
    [DataField]
    public ProtoId<EmotePrototype>? EmoteId = null;
    // Exodus-Kidans-End
}
