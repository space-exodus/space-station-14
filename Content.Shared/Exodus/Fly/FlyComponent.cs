using Content.Shared.Fluids.Components;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Utility;

namespace Content.Shared.Exodus.Fly;

/// <summary>
/// Marks an entity as able to flying
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class FlyComponent : Component
{
    /// <summary>
    /// Effect entity to delete upon removing the component. Only matters clientside.
    /// </summary>
    [ViewVariables]
    public EntityUid Effect = EntityUid.Invalid;


    [ViewVariables(VVAccess.ReadWrite), DataField]
    public bool DoAnimation = false;

    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan AnimationTimeEnd = TimeSpan.Zero;


    [ViewVariables(VVAccess.ReadWrite), DataField]
    public float LandAngle = (float) - Math.PI / 6;

    [ViewVariables(VVAccess.ReadWrite), DataField]
    public float TakeoffAngle = (float) Math.PI / 6;


    [ViewVariables(VVAccess.ReadWrite), DataField]
    public float LandTime = 0.5f;

    [ViewVariables(VVAccess.ReadWrite), DataField]
    public float TakeoffTime = 0.5f;


    [ViewVariables(VVAccess.ReadWrite), DataField("inAir")]
    public bool IsInAir = false;

    [DataField("groundEffectRsi")]
    public ResPath? GroundEffectRsi = null;


    [ViewVariables(VVAccess.ReadWrite), DataField("soundTakeoff")]
    public SoundSpecifier? SoundTakeoff = new SoundPathSpecifier("/Audio/Items/Mining/fultext_launch.ogg");

    [ViewVariables(VVAccess.ReadWrite), DataField("soundLand")]
    public SoundSpecifier? SoundLand = new SoundPathSpecifier("/Audio/Items/Mining/fultext_launch.ogg");
}
