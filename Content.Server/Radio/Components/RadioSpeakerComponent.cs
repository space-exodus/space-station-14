using Content.Server.Radio.EntitySystems;
using Content.Shared.Chat;
using Content.Server.Chat.Systems;
using ChatChannel = Content.Server.Chat.Systems.InGameICChatType;
using Content.Shared.Radio;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.Radio.Components;

/// <summary>
///     Listens for radio messages and relays them to local chat.
/// </summary>
[RegisterComponent]
[Access(typeof(RadioDeviceSystem))]
public sealed partial class RadioSpeakerComponent : Component
{
    /// <summary>
    /// Whether or not interacting with this entity
    /// toggles it on or off.
    /// </summary>
    [DataField("toggleOnInteract")]
    public bool ToggleOnInteract = true;

    [DataField("channels", customTypeSerializer: typeof(PrototypeIdHashSetSerializer<RadioChannelPrototype>))]
    public HashSet<string> Channels = new () { SharedChatSystem.CommonChannel };

    [DataField("enabled")]
    public bool Enabled;

    // Exodus - Advanced speak control - start
    [DataField]
    public bool HideSpeaker = false;
    // Exodus - Advanced speak control - end

}
