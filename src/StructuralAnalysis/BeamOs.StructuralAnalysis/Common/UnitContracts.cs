using System.Diagnostics.CodeAnalysis;

namespace BeamOs.StructuralAnalysis.Contracts.Common;

public readonly record struct Length
{
    public required double Value { get; init; }
    public required LengthUnitContract Unit { get; init; }

    public Length() { }

    [SetsRequiredMembers]
    public Length(double value, LengthUnitContract unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}

public readonly record struct Area
{
    public required double Value { get; init; }
    public required AreaUnitContract Unit { get; init; }

    public Area() { }

    [SetsRequiredMembers]
    public Area(double value, AreaUnitContract unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}

public readonly record struct Ratio
{
    public required double Value { get; init; }
    public required RatioUnitContract Unit { get; init; }

    public Ratio() { }

    [SetsRequiredMembers]
    public Ratio(double value, RatioUnitContract unit)
    {
        this.Value = value;
        this.Unit = unit;
    }

    public double As(RatioUnitContract targetUnit)
    {
        return this.Unit switch
        {
            RatioUnitContract.DecimalFraction
                when targetUnit == RatioUnitContract.DecimalFraction => this.Value,
            RatioUnitContract.DecimalFraction when targetUnit == RatioUnitContract.Percent =>
                this.Value * 100,
            RatioUnitContract.Percent when targetUnit == RatioUnitContract.Percent => this.Value,
            RatioUnitContract.Percent when targetUnit == RatioUnitContract.DecimalFraction =>
                this.Value / 100,
            RatioUnitContract.Undefined or _ => throw new NotSupportedException(
                $"Conversion from {this.Unit} to {targetUnit} is not supported."
            ),
        };
    }
}

public readonly record struct Volume
{
    public required double Value { get; init; }
    public required VolumeUnitContract Unit { get; init; }

    public Volume() { }

    [SetsRequiredMembers]
    public Volume(double value, VolumeUnitContract unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}

public readonly record struct AreaMomentOfInertia
{
    public required double Value { get; init; }
    public required AreaMomentOfInertiaUnitContract Unit { get; init; }

    public AreaMomentOfInertia() { }

    [SetsRequiredMembers]
    public AreaMomentOfInertia(double value, AreaMomentOfInertiaUnitContract unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}

public readonly record struct Force
{
    public required double Value { get; init; }
    public required ForceUnitContract Unit { get; init; }

    public Force() { }

    [SetsRequiredMembers]
    public Force(double value, ForceUnitContract unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}

public readonly record struct Angle
{
    public required double Value { get; init; }
    public required AngleUnitContract Unit { get; init; }

    [SetsRequiredMembers]
    public Angle()
        : this(0, AngleUnitContract.Radian) { }

    [SetsRequiredMembers]
    public Angle(double value, AngleUnitContract unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}

public readonly record struct Torque
{
    public required double Value { get; init; }
    public required TorqueUnitContract Unit { get; init; }

    public Torque() { }

    [SetsRequiredMembers]
    public Torque(double value, TorqueUnitContract unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}

public readonly record struct ForcePerLength
{
    public required double Value { get; init; }
    public required ForcePerLengthUnitContract Unit { get; init; }

    public ForcePerLength() { }

    [SetsRequiredMembers]
    public ForcePerLength(double value, ForcePerLengthUnitContract unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}

public readonly record struct Pressure
{
    public required double Value { get; init; }
    public required PressureUnitContract Unit { get; init; }

    public Pressure() { }

    [SetsRequiredMembers]
    public Pressure(double value, PressureUnitContract unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}
