// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Robust.Shared.Timing;

namespace Content.Shared.Exodus.Chat.Channels.Emote.Animation;

[ImplicitDataDefinitionForInheritors, Access(typeof(SharedEmoteAnimationSystem))]
public abstract partial class EmoteAnimation
{
    #region DataDefinition

    /// <summary>
    /// Delay in seconds before animation will start
    /// </summary>
    [DataField]
    public float Delay = 0f;

    /// <summary>
    /// Animation playtime in seconds 
    /// </summary>
    [DataField]
    public float Playtime = 0f;

    #endregion

    [Dependency] protected readonly GameTiming Timing = default!;

    public EmoteAnimation()
    {
        IoCManager.InjectDependencies(this);
    }

    #region Life-Cycle

    public virtual void Start(Entity<EmoteAnimationComponent> mob) { }
    public virtual void Update(Entity<EmoteAnimationComponent> mob, float deltaTime) { }
    public virtual void End(Entity<EmoteAnimationComponent> mob) { }

    #endregion
}
