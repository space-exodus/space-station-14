// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Server.Jittering;
using Content.Shared.Exodus.Chat.Channels.Emote.Animation;

namespace Content.Server.Exodus.Chat.Channels.Emote.Animation;

public sealed partial class EmoteJitterAnimation : EmoteAnimation
{
    [DataField]
    public float Amplitude = 5;

    [DataField]
    public float Frequency = 2;

    [Dependency] private IEntitySystemManager _system = default!;

    private JitteringSystem? _jittering;

    public override void Start(Entity<EmoteAnimationComponent> mob)
    {
        base.Start(mob);

        _jittering ??= _system.GetEntitySystem<JitteringSystem>();

        _jittering.DoJitter(mob, TimeSpan.FromSeconds(Playtime), true, Amplitude, Frequency);
    }

    public override void End(Entity<EmoteAnimationComponent> mob)
    {
        base.End(mob);

        // cannot be just interrupted due to specifics of JitteringSystem work
    }
}
