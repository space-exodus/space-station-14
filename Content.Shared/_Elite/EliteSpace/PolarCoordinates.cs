using System.Numerics;
using Robust.Shared.Serialization;

namespace Content.Shared.Elite.EliteSpace;

[DataDefinition, Serializable, NetSerializable]
public partial struct PolarCoordinates
{
    [DataField]
    public float Radius { get; set; }

    [DataField]
    public Angle Angle { get; set; }

    public PolarCoordinates(float radius, Angle angle)
    {
        Radius = radius;
        Angle = angle;
    }

    public Vector2 ToCartesian() => Angle.RotateVec(new Vector2(Radius, 0));
}
