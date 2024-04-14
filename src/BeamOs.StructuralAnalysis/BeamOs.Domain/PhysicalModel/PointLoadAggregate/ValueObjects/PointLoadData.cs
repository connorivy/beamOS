using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.Common.Models;
using MathNet.Spatial.Euclidean;
using UnitsNet;

namespace BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;

public class PointLoadData : BeamOSValueObject
{
    public PointLoadData(Force force, Vector3D normalizedDirection)
    {
        this.Force = force;
        this.NormalizedDirection = normalizedDirection;
    }

    public Force Force { get; private set; }
    public Vector3D NormalizedDirection { get; private set; }

    public Force GetForceInDirection(CoordinateSystemDirection3D direction)
    {
        return direction switch
        {
            CoordinateSystemDirection3D.AlongX => this.Force * this.NormalizedDirection.X,
            CoordinateSystemDirection3D.AlongY => this.Force * this.NormalizedDirection.Y,
            CoordinateSystemDirection3D.AlongZ => this.Force * this.NormalizedDirection.Z,
            CoordinateSystemDirection3D.AboutX
            or CoordinateSystemDirection3D.AboutY
            or CoordinateSystemDirection3D.AboutZ
                => throw new ArgumentException("Point load has no force about an axis"),
            CoordinateSystemDirection3D.Undefined
                => throw new ArgumentException("Unexpected value for direction, Undefined"),
            _ => throw new NotImplementedException(),
        };
    }

    public Force GetForceInDirection(Vector3D direction)
    {
        // magnitude of projection of A onto B = (A . B) / | B |
        double magnitudeOfProjection =
            this.NormalizedDirection.DotProduct(direction) / direction.Length;
        return this.Force * magnitudeOfProjection;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Force;
        yield return this.NormalizedDirection;
    }
}
