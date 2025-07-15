using Robust.Shared.GameStates;

namespace Content.Shared.Exodus.Movement.Components;

[NetworkedComponent, RegisterComponent]
[Access(typeof(HandheldFrictionSystem))]
public sealed partial class HandheldFrictionComponent : Component
{
    [DataField]
    public float Friction = 0.125f;

    [DataField]
    public float FrictionNoInput = 0.125f;

    [DataField]
    public float Acceleration = 0.25f;
}
