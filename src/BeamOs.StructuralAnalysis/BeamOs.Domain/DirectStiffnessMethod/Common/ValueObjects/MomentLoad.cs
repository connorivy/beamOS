using BeamOs.Domain.Common.Models;
using MathNet.Numerics.LinearAlgebra;
using UnitsNet;

namespace BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;

public class MomentLoad : BeamOSValueObject
{
    public MomentLoad(Torque torque, Vector<double> axisDirection)
    {
        this.Torque = torque;
        this.NormalizedAxisDirection = axisDirection.Normalize(2);
    }

    public Torque Torque { get; set; }
    public Vector<double> NormalizedAxisDirection { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Torque;
        yield return this.NormalizedAxisDirection;
    }
}
