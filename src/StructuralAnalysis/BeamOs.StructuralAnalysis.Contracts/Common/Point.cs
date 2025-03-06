using System.Diagnostics.CodeAnalysis;

namespace BeamOs.StructuralAnalysis.Contracts.Common;

public readonly record struct Point
{
    public required double X { get; init; }
    public required double Y { get; init; }
    public required double Z { get; init; }
    public required LengthUnit LengthUnit { get; init; }

    public Point() { }

    [SetsRequiredMembers]
    public Point(double x, double y, double z, LengthUnit lengthUnit)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
        this.LengthUnit = lengthUnit;
    }

    [SetsRequiredMembers]
    public Point(Length x, Length y, Length z)
        : this(x.Value, y.Value, z.Value, x.Unit)
    {
        if (x.Unit != y.Unit || x.Unit != z.Unit)
        {
            throw new InvalidOperationException("Cannot mix units");
        }
    }
}

[method: SetsRequiredMembers]
public readonly struct PartialPoint(double? x, double? y, double? z, LengthUnit lengthUnit)
{
    public double? X { get; init; } = x;
    public double? Y { get; init; } = y;
    public double? Z { get; init; } = z;
    public required LengthUnit LengthUnit { get; init; } = lengthUnit;
}

public readonly record struct Vector3
{
    public required double X { get; init; }
    public required double Y { get; init; }
    public required double Z { get; init; }

    public Vector3() { }

    [SetsRequiredMembers]
    public Vector3(double x, double y, double z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }
}
