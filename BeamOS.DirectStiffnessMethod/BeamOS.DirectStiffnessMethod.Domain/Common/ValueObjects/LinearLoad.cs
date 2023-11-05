using BeamOS.Common.Domain.Models;
using MathNet.Numerics.LinearAlgebra;
using UnitsNet;

namespace BeamOS.DirectStiffnessMethod.Domain.Common.ValueObjects;
public class LinearLoad : BeamOSValueObject
{
    public LinearLoad(Force force, Vector<double> direction)
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
}
