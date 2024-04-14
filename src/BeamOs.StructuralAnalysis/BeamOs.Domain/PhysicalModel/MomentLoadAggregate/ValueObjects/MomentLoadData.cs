using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.Common.Models;
using MathNet.Numerics.LinearAlgebra;
using UnitsNet;

namespace BeamOs.Domain.PhysicalModel.MomentLoadAggregate.ValueObjects;

public sealed class MomentLoadData : BeamOSValueObject
{
    public MomentLoadData(Torque torque, Vector<double> axisDirection)
    {
        this.Torque = torque;
        this.NormalizedAxisDirection = axisDirection.Normalize(2);
    }

    public Torque Torque { get; private set; }
    public Vector<double> NormalizedAxisDirection { get; private set; }

    public Torque GetTorqueInDirection(CoordinateSystemDirection3D direction)
    {
        return direction switch
        {
            CoordinateSystemDirection3D.AlongX
            or CoordinateSystemDirection3D.AlongY
            or CoordinateSystemDirection3D.AlongZ
                => throw new ArgumentException("Moment load has no torque along an axis"),
            CoordinateSystemDirection3D.AboutX => this.Torque * this.NormalizedAxisDirection[0],
            CoordinateSystemDirection3D.AboutY => this.Torque * this.NormalizedAxisDirection[1],
            CoordinateSystemDirection3D.AboutZ => this.Torque * this.NormalizedAxisDirection[2],
            CoordinateSystemDirection3D.Undefined
                => throw new ArgumentException("Unexpected value for direction, Undefined"),
            _ => throw new NotImplementedException(),
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Torque;
        yield return this.NormalizedAxisDirection;
    }
}
