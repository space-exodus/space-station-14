// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Robust.Shared.GameStates;

namespace Content.Shared.Exodus.Chat.Channels.Emote.Animation;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(SharedEmoteAnimationSystem))]
public sealed partial class EmoteAnimationComponent : Component
{
    [DataField, AutoNetworkedField]
    public EmotePrototype? CurrentEmote;

    [DataField, AutoNetworkedField]
    public float AnimationTiming = 0f;

    public EmoteAnimation? Animation => CurrentEmote?.Animation;
}
