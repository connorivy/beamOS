using BeamOs.Common.Domain.Models;
using MathNet.Spatial.Euclidean;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Domain.Common.ValueObjects;

public class Line : BeamOSValueObject
{
    public Line(Point startPoint, Point endPoint)
    {
        this.StartPoint = startPoint;
        this.EndPoint = endPoint;
        this.Length = GetLength(this.StartPoint, this.EndPoint);
    }

    public Line(
        double x0,
        double y0,
        double z0,
        double x1,
        double y1,
        double z1,
        LengthUnit lengthUnit
    )
    {
        this.StartPoint = new Point(x0, y0, z0, lengthUnit);
        this.EndPoint = new Point(x1, y1, z1, lengthUnit);
        this.Length = GetLength(this.StartPoint, this.EndPoint);
    }

    public Point StartPoint { get; }
    public Point EndPoint { get; }
    public Length Length { get; }

    public static Length GetLength(Point startPoint, Point endPoint)
    {
        var deltaX = endPoint.XCoordinate - startPoint.XCoordinate;
        var deltaY = endPoint.YCoordinate - startPoint.YCoordinate;
        var deltaZ = endPoint.ZCoordinate - startPoint.ZCoordinate;
        var squared = deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ;
        return new Length(Math.Sqrt(squared.SquareMeters), LengthUnit.Meter);
    }

    public Vector3D GetDirection(LengthUnit lengthUnit)
    {
        return new(
            (this.EndPoint.XCoordinate - this.StartPoint.XCoordinate).As(lengthUnit),
            (this.EndPoint.YCoordinate - this.StartPoint.YCoordinate).As(lengthUnit),
            (this.EndPoint.ZCoordinate - this.StartPoint.ZCoordinate).As(lengthUnit)
        );
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.StartPoint;
        yield return this.EndPoint;
    }
}
