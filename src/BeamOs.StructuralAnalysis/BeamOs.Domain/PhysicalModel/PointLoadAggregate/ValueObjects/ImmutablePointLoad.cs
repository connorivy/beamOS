using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.Common.Models;
using MathNet.Numerics.LinearAlgebra;
using UnitsNet;

namespace BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;

public class ImmutablePointLoad : BeamOSValueObject
{
    public ImmutablePointLoad(Force force, Vector<double> normalizedDirection)
    {
        this.Force = force;
        this.NormalizedDirection = normalizedDirection;
    }

    public Force Force { get; private set; }
    public Vector<double> NormalizedDirection { get; private set; }

    public Force GetForceInDirection(CoordinateSystemDirection3D direction)
    {
        return direction switch
        {
            CoordinateSystemDirection3D.AlongX => this.Force * this.NormalizedDirection[0],
            CoordinateSystemDirection3D.AlongY => this.Force * this.NormalizedDirection[1],
            CoordinateSystemDirection3D.AlongZ => this.Force * this.NormalizedDirection[2],
            CoordinateSystemDirection3D.AboutX
            or CoordinateSystemDirection3D.AboutY
            or CoordinateSystemDirection3D.AboutZ
                => throw new ArgumentException("Point load has no force about an axis"),
            CoordinateSystemDirection3D.Undefined
                => throw new ArgumentException("Unexpected value for direction, Undefined"),
            _ => throw new NotImplementedException(),
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Force;
        yield return this.NormalizedDirection;
    }
}
