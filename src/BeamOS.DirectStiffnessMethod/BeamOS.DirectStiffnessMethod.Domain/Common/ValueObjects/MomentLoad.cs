using BeamOS.Common.Domain.Models;
using MathNet.Numerics.LinearAlgebra;
using UnitsNet;

namespace BeamOS.DirectStiffnessMethod.Domain.Common.ValueObjects;

public class MomentLoad : BeamOSValueObject
{
    public MomentLoad(Torque magnitude, Vector<double> axisDirection)
    {
        this.Magnitude = magnitude;
        this.NormalizedAxisDirection = axisDirection.Normalize(2);
    }

    public Torque Magnitude { get; set; }
    public Vector<double> NormalizedAxisDirection { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Magnitude;
        yield return this.NormalizedAxisDirection;
    }
}
