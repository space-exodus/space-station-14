// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Server.Wagging;
using Content.Shared.Exodus.Chat.Channels.Emote.Animation;

namespace Content.Server.Exodus.Chat.Channels.Emote.Animation;

public sealed partial class EmoteWaggingAnimation : EmoteAnimation
{
    [Dependency] private readonly IEntitySystemManager _system = default!;

    private WaggingSystem? _wagging;

    public override void Start(Entity<EmoteAnimationComponent> mob)
    {
        base.Start(mob);

        _wagging ??= _system.GetEntitySystem<WaggingSystem>();
        _wagging.TryToggleWagging(mob, true);
    }

    public override void End(Entity<EmoteAnimationComponent> mob)
    {
        base.End(mob);

        _wagging ??= _system.GetEntitySystem<WaggingSystem>();
        _wagging.TryToggleWagging(mob, false);
    }
}
