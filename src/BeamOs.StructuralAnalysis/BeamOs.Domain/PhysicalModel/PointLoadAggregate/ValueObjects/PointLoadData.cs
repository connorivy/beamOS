using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.Common.Models;
using MathNet.Spatial.Euclidean;
using UnitsNet;

namespace BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;

public class PointLoadData : BeamOSValueObject
{
    public PointLoadData(Force force, Vector3D direction)
    {
        this.Force = force;
        this.Direction = direction;
    }

    public Force Force { get; private set; }
    public Vector3D Direction { get; private set; }

    public Force GetForceInDirection(CoordinateSystemDirection3D direction)
    {
        return direction switch
        {
            CoordinateSystemDirection3D.AlongX => this.Force * this.Direction.X,
            CoordinateSystemDirection3D.AlongY => this.Force * this.Direction.Y,
            CoordinateSystemDirection3D.AlongZ => this.Force * this.Direction.Z,
            CoordinateSystemDirection3D.AboutX
            or CoordinateSystemDirection3D.AboutY
            or CoordinateSystemDirection3D.AboutZ
                => throw new ArgumentException("Point load has no force about an axis"),
            CoordinateSystemDirection3D.Undefined
                => throw new ArgumentException("Unexpected value for direction, Undefined"),
            _ => throw new NotImplementedException(),
        };
    }

    public Force GetForceInLocalAxisDirection(LinearCoordinateDirection3D direction)
    {
        return direction switch
        {
            LinearCoordinateDirection3D.AlongX => this.Force * this.Direction.X,
            LinearCoordinateDirection3D.AlongY => this.Force * this.Direction.Y,
            LinearCoordinateDirection3D.AlongZ => this.Force * this.Direction.Z,
            LinearCoordinateDirection3D.Undefined
                => throw new ArgumentException("Unexpected value for direction, Undefined"),
            _ => throw new NotImplementedException(),
        };
    }

    public Force GetForceInDirection(Vector3D direction)
    {
        // magnitude of projection of A onto B = (A . B) / | B |
        double magnitudeOfProjection = this.Direction.DotProduct(direction) / direction.Length;
        return this.Force * magnitudeOfProjection;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Force;
        yield return this.Direction;
    }
}
