using Content.Server.Chat.Systems;
using Content.Shared.Chat;
using Content.Shared.Chat.Prototypes; // Exodus-Kidans
using Content.Shared.Radio;
using Robust.Shared.Prototypes; // Exodus-Kidans
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Server.Radio.Components;

/// <summary>
///     This component allows an entity to directly translate spoken text into radio messages (effectively an intrinsic
///     radio headset).
/// </summary>
[RegisterComponent]
public sealed partial class IntrinsicRadioTransmitterComponent : Component
{
    [DataField("channels", customTypeSerializer: typeof(PrototypeIdHashSetSerializer<RadioChannelPrototype>))]
    public HashSet<string> Channels = new() { SharedChatSystem.CommonChannel };

    // Exodus-Kidans-Start
    /// <summary>
    /// Emote sent by entity when it speaks. Does nothing when nothing specified.
    /// </summary>
    [DataField]
    public ProtoId<EmotePrototype>? EmoteId = null;
    // Exodus-Kidans-End
}
