using Content.Shared.Fluids.Components;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared.Exodus.Fly;

/// <summary>
/// Marks an entity as able to flying
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class FlyComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite), DataField, AutoNetworkedField]
    public bool DoAnimation = false;

    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan AnimationTimeEnd = TimeSpan.Zero;


    [ViewVariables(VVAccess.ReadWrite), DataField]
    public float LandAngle = (float) Math.PI / 6;

    [ViewVariables(VVAccess.ReadWrite), DataField]
    public float TakeoffAngle = (float) Math.PI / 6;


    [ViewVariables(VVAccess.ReadWrite), DataField]
    public float LandTime = 0.5f;

    [ViewVariables(VVAccess.ReadWrite), DataField]
    public float TakeoffTime = 0.5f;


    [ViewVariables(VVAccess.ReadWrite), DataField("inAir")]
    public bool IsInAir = false;



    [ViewVariables(VVAccess.ReadWrite), DataField("sound"), AutoNetworkedField]
    public SoundSpecifier? SoundTakeoff = new SoundPathSpecifier("/Audio/Items/Mining/fultext_launch.ogg");

    [ViewVariables(VVAccess.ReadWrite), DataField("sound"), AutoNetworkedField]
    public SoundSpecifier? SoundLanding = new SoundPathSpecifier("/Audio/Items/Mining/fultext_launch.ogg");
}
