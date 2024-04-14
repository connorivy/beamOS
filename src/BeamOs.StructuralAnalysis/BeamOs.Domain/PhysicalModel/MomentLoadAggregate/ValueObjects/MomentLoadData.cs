using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.Common.Models;
using MathNet.Spatial.Euclidean;
using UnitsNet;

namespace BeamOs.Domain.PhysicalModel.MomentLoadAggregate.ValueObjects;

public sealed class MomentLoadData : BeamOSValueObject
{
    public MomentLoadData(Torque torque, UnitVector3D normalizedAxisDirection)
    {
        this.Torque = torque;
        this.NormalizedAxisDirection = normalizedAxisDirection;
    }

    public MomentLoadData(Torque torque, Vector3D axisDirection)
        : this(torque, axisDirection.Normalize()) { }

    public Torque Torque { get; private set; }
    public UnitVector3D NormalizedAxisDirection { get; private set; }

    public Torque GetTorqueInDirection(CoordinateSystemDirection3D direction)
    {
        return direction switch
        {
            CoordinateSystemDirection3D.AlongX
            or CoordinateSystemDirection3D.AlongY
            or CoordinateSystemDirection3D.AlongZ
                => throw new ArgumentException("Moment load has no torque along an axis"),
            CoordinateSystemDirection3D.AboutX => this.Torque * this.NormalizedAxisDirection.X,
            CoordinateSystemDirection3D.AboutY => this.Torque * this.NormalizedAxisDirection.Y,
            CoordinateSystemDirection3D.AboutZ => this.Torque * this.NormalizedAxisDirection.Z,
            CoordinateSystemDirection3D.Undefined
                => throw new ArgumentException("Unexpected value for direction, Undefined"),
            _ => throw new NotImplementedException(),
        };
    }

    public Torque GetTorqueAboutAxis(Vector3D direction)
    {
        return this.GetTorqueAboutAxis(direction.Normalize());
    }

    public Torque GetTorqueAboutAxis(UnitVector3D direction)
    {
        // magnitude of projection of A onto B = (A . B) / | B |
        double magnitude = this.NormalizedAxisDirection.DotProduct(direction);
        return this.Torque * magnitude;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Torque;
        yield return this.NormalizedAxisDirection;
    }
}
