using System.Numerics;
using BeamOs.Common.Domain.Models;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.StructuralAnalysis.Domain.Common;

public class Point : BeamOSValueObject
{
    public Point(double x, double y, double z, LengthUnit lengthUnit)
    {
        this.X = new Length(x, lengthUnit);
        this.Y = new Length(y, lengthUnit);
        this.Z = new Length(z, lengthUnit);
    }

    public Point(Length x, Length y, Length z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public Length X { get; private set; }
    public Length Y { get; private set; }
    public Length Z { get; private set; }

    public static Point operator -(Point lhs, Point rhs) =>
        new(lhs.X - rhs.X, lhs.Y - rhs.Y, rhs.Z - rhs.Z);

    public double CalculateDistance(Length x1, Length y1, Length z1)
    {
        return Math.Sqrt(
            Math.Pow(this.X.Meters - x1.Meters, 2)
                + Math.Pow(this.Y.Meters - y1.Meters, 2)
                + Math.Pow(this.Z.Meters - z1.Meters, 2)
        );
    }

    public Length ShortestDistanceToLine(Line line) =>
        this.ShortestDistanceToLine(line.StartPoint, line.EndPoint);

    public Length ShortestDistanceToLine(Point startPoint, Point endPoint)
    {
        // Calculate the distance from this point to the line defined by two points
        double numerator = Math.Abs(
            (endPoint.Y.Meters - startPoint.Y.Meters) * this.X.Meters
                - (endPoint.X.Meters - startPoint.X.Meters) * this.Y.Meters
                + endPoint.X.Meters * startPoint.Y.Meters
                - endPoint.Y.Meters * startPoint.X.Meters
        );
        double denominator = Math.Sqrt(
            Math.Pow(endPoint.Y.Meters - startPoint.Y.Meters, 2)
                + Math.Pow(endPoint.X.Meters - startPoint.X.Meters, 2)
        );
        return new Length(numerator / denominator, LengthUnit.Meter);
    }

    public static implicit operator Vector3(Point point)
    {
        return new Vector3((float)point.X.Meters, (float)point.Y.Meters, (float)point.Z.Meters);
    }

    public static implicit operator Point(Vector3 vector)
    {
        return new Point(
            new Length(vector.X, LengthUnit.Meter),
            new Length(vector.Y, LengthUnit.Meter),
            new Length(vector.Z, LengthUnit.Meter)
        );
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.X;
        yield return this.Y;
        yield return this.Z;
    }
}
