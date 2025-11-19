// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Robust.Shared.Utility;

namespace Content.Shared.Exodus.Chat.Channels.Emote.Animation;

// EXODUS-TODO: Rewrite this piece of shit
/// What? Isn't we doing complete rewrite to get rid of shitcode? Yeah, that's true
/// But this is the case where I just get really bored. My main task right now is to just make EmoteAnimationSystem work
/// This system was built as a singleton over EmoteSystem and can be easily deleted in few clicks without any consequences
/// This also means that this shitcode is localized and will not be growing over codebase
/// I will do a complete rewrite of this system in the later stages of NewChat PR

public abstract partial class SharedEmoteAnimationSystem : EntitySystem
{
    private HashSet<Entity<EmoteAnimationComponent>> _active = new(); // keep in mind that it will be quite different for server side and clients side

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<EmoteAnimationComponent, EmoteEvent>(OnEmote);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var active = _active;
        _active = new();

        foreach (var emoting in active)
        {
            if (emoting.Comp.Deleted) continue;

            DebugTools.Assert(emoting.Comp.Animation is not null, "Got emote without animation in _active list");

            emoting.Comp.AnimationTiming += frameTime;
            emoting.Comp.Animation.Update(emoting, frameTime);

            if (emoting.Comp.AnimationTiming > emoting.Comp.Animation.Playtime)
            {
                EndAnimation((emoting, emoting.Comp));
                continue;
            }

            _active.Add(emoting);
        }
    }

    private void OnEmote(Entity<EmoteAnimationComponent> entity, ref EmoteEvent emote)
    {
        if (emote.Emote == null)
            return;

        ExecuteAnimation((entity, entity.Comp), emote.Emote);
    }

    public void ExecuteAnimation(Entity<EmoteAnimationComponent?> entity, EmotePrototype emote)
    {
        DebugTools.Assert(emote.Animation is not null, $"Tried to execute animation of \"{emote.ID}\" without any animation");

        if (!Resolve(entity, ref entity.Comp))
            return;

        if (entity.Comp.Animation is not null)
        {
            EndAnimation(entity);
            _active.Remove((entity, entity.Comp));
        }

        entity.Comp.CurrentEmote = emote;
        entity.Comp.AnimationTiming = 0;

        emote.Animation.Start((entity, entity.Comp));

        _active.Add((entity, entity.Comp));
    }

    public void EndAnimation(Entity<EmoteAnimationComponent?> entity)
    {
        if (!Resolve(entity, ref entity.Comp))
            return;

        if (entity.Comp.CurrentEmote is null)
            return;

        if (entity.Comp.Animation is null)
            return;

        entity.Comp.Animation.End((entity, entity.Comp));
        entity.Comp.CurrentEmote = null;
    }
}
