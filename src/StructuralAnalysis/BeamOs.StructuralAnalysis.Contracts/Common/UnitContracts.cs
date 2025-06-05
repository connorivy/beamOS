using System.Diagnostics.CodeAnalysis;

namespace BeamOs.StructuralAnalysis.Contracts.Common;

public readonly record struct Length
{
    public required double Value { get; init; }
    public required LengthUnit Unit { get; init; }

    public Length() { }

    [SetsRequiredMembers]
    public Length(double value, LengthUnit unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}

public readonly record struct Area
{
    public required double Value { get; init; }
    public required AreaUnit Unit { get; init; }

    public Area() { }

    [SetsRequiredMembers]
    public Area(double value, AreaUnit unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}

public readonly record struct Ratio
{
    public required double Value { get; init; }
    public required RatioUnit Unit { get; init; }

    public Ratio() { }

    [SetsRequiredMembers]
    public Ratio(double value, RatioUnit unit)
    {
        this.Value = value;
        this.Unit = unit;
    }

    public double As(RatioUnit targetUnit)
    {
        return this.Unit switch
        {
            RatioUnit.DecimalFraction when targetUnit == RatioUnit.DecimalFraction => this.Value,
            RatioUnit.DecimalFraction when targetUnit == RatioUnit.Percent => this.Value * 100,
            RatioUnit.Percent when targetUnit == RatioUnit.DecimalFraction => this.Value,
            RatioUnit.Percent when targetUnit == RatioUnit.DecimalFraction => this.Value / 100,
            RatioUnit.Undefined or _ => throw new NotSupportedException(
                $"Conversion from {this.Unit} to {targetUnit} is not supported."
            ),
        };
    }
}

public readonly record struct Volume
{
    public required double Value { get; init; }
    public required VolumeUnit Unit { get; init; }

    public Volume() { }

    [SetsRequiredMembers]
    public Volume(double value, VolumeUnit unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}

public readonly record struct AreaMomentOfInertia
{
    public required double Value { get; init; }
    public required AreaMomentOfInertiaUnit Unit { get; init; }

    public AreaMomentOfInertia() { }

    [SetsRequiredMembers]
    public AreaMomentOfInertia(double value, AreaMomentOfInertiaUnit unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}

public readonly record struct Force
{
    public required double Value { get; init; }
    public required ForceUnit Unit { get; init; }

    public Force() { }

    [SetsRequiredMembers]
    public Force(double value, ForceUnit unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}

public readonly record struct Angle
{
    public required double Value { get; init; }
    public required AngleUnit Unit { get; init; }

    [SetsRequiredMembers]
    public Angle()
        : this(0, AngleUnit.Radian) { }

    [SetsRequiredMembers]
    public Angle(double value, AngleUnit unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}

public readonly record struct Torque
{
    public required double Value { get; init; }
    public required TorqueUnit Unit { get; init; }

    public Torque() { }

    [SetsRequiredMembers]
    public Torque(double value, TorqueUnit unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}

public readonly record struct ForcePerLength
{
    public required double Value { get; init; }
    public required ForcePerLengthUnit Unit { get; init; }

    public ForcePerLength() { }

    [SetsRequiredMembers]
    public ForcePerLength(double value, ForcePerLengthUnit unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}

public readonly record struct Pressure
{
    public required double Value { get; init; }
    public required PressureUnit Unit { get; init; }

    public Pressure() { }

    [SetsRequiredMembers]
    public Pressure(double value, PressureUnit unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}
