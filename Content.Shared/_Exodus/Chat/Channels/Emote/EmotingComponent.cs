// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Exodus.Chat.Channels.Emote;

[RegisterComponent, NetworkedComponent, Access(typeof(SharedEmoteSystem))]
public sealed partial class EmotingComponent : Component
{
    [DataField]
    public HashSet<EmotePrototype> AvailableEmotes = new();
}

[Serializable, NetSerializable]
public sealed partial class EmotingComponentState : ComponentState
{
    public HashSet<ProtoId<EmotePrototype>> AvailableEmotes = new();
}
