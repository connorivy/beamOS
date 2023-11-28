using BeamOS.Common.Domain.Models;
using MathNet.Numerics.LinearAlgebra;
using UnitsNet;

namespace BeamOS.DirectStiffnessMethod.Domain.Common.ValueObjects;
public class PointLoad : BeamOSValueObject
{
    public PointLoad(Force force, Vector<double> direction)
    {
        this.Magnitude = force;
        this.NormalizedDirection = direction.Normalize(2);
    }

    public Force Magnitude { get; set; }
    public Vector<double> NormalizedDirection { get; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Magnitude;
        yield return this.NormalizedDirection;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private PointLoad() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
