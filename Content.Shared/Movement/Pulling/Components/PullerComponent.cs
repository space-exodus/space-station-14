using Content.Shared.Movement.Pulling.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared.Movement.Pulling.Components;

/// <summary>
/// Specifies an entity as being able to pull another entity with <see cref="PullableComponent"/>
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(PullingSystem))]
public sealed partial class PullerComponent : Component
{
    // My raiding guild
    /// <summary>
    /// Next time the puller can throw what is being pulled.
    /// Used to avoid spamming it for infinite spin + velocity.
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoNetworkedField]
    public TimeSpan NextThrow;

    [DataField]
    public TimeSpan ThrowCooldown = TimeSpan.FromSeconds(1);

    // Exodus-RefactorPullerModificators-Start
    // Before changing how this is updated, please see SharedPullerSystem.RefreshMovementSpeed
    [DataField]
    public float WalkSpeedModifier = 0.95f;

    [DataField]
    public float SprintSpeedModifier = 0.95f;
    // Exodus-RefactorPullerModificators-End

    /// <summary>
    /// Entity currently being pulled if applicable.
    /// </summary>
    [AutoNetworkedField, DataField]
    public EntityUid? Pulling;

    /// <summary>
    ///     Does this entity need hands to be able to pull something?
    /// </summary>
    [DataField]
    public bool NeedsHands = true;
}
