using System.Diagnostics.CodeAnalysis;

namespace BeamOs.Contracts.Common;

public record UnitValueDto(double Value, string Unit);

public abstract record UnitValueContract<TUnit>
    where TUnit : Enum
{
    public required double Value { get; init; }
    public required TUnit Unit { get; init; }

    protected UnitValueContract() { }

    [SetsRequiredMembers]
    protected UnitValueContract(double value, TUnit unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}

public readonly record struct LengthContract
{
    public required double Value { get; init; }
    public required LengthUnitContract Unit { get; init; }

    public LengthContract() { }

    [SetsRequiredMembers]
    public LengthContract(double value, LengthUnitContract unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}

public readonly record struct AreaContract
{
    public required double Value { get; init; }
    public required AreaUnitContract Unit { get; init; }

    public AreaContract() { }

    [SetsRequiredMembers]
    public AreaContract(double value, AreaUnitContract unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}

public readonly record struct VolumeContract
{
    public required double Value { get; init; }
    public required VolumeUnitContract Unit { get; init; }

    public VolumeContract() { }

    [SetsRequiredMembers]
    public VolumeContract(double value, VolumeUnitContract unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}

public readonly record struct AreaMomentOfInertiaContract
{
    public required double Value { get; init; }
    public required AreaMomentOfInertiaUnitContract Unit { get; init; }

    public AreaMomentOfInertiaContract() { }

    [SetsRequiredMembers]
    public AreaMomentOfInertiaContract(double value, AreaMomentOfInertiaUnitContract unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}

public readonly record struct ForceContract
{
    public required double Value { get; init; }
    public required ForceUnitContract Unit { get; init; }

    public ForceContract() { }

    [SetsRequiredMembers]
    public ForceContract(double value, ForceUnitContract unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}

public readonly record struct AngleContract
{
    public required double Value { get; init; }
    public required AngleUnitContract Unit { get; init; }

    public AngleContract() { }

    [SetsRequiredMembers]
    public AngleContract(double value, AngleUnitContract unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}

public readonly record struct TorqueContract
{
    public required double Value { get; init; }
    public required TorqueUnitContract Unit { get; init; }

    public TorqueContract() { }

    [SetsRequiredMembers]
    public TorqueContract(double value, TorqueUnitContract unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}

public readonly record struct ForcePerLengthContract
{
    public required double Value { get; init; }
    public required ForcePerLengthUnitContract Unit { get; init; }

    public ForcePerLengthContract() { }

    [SetsRequiredMembers]
    public ForcePerLengthContract(double value, ForcePerLengthUnitContract unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}

public readonly record struct PressureContract
{
    public required double Value { get; init; }
    public required PressureUnitContract Unit { get; init; }

    public PressureContract() { }

    [SetsRequiredMembers]
    public PressureContract(double value, PressureUnitContract unit)
    {
        this.Value = value;
        this.Unit = unit;
    }
}
