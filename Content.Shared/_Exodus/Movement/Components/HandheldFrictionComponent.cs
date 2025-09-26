// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt
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
