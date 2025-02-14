using System.Diagnostics.CodeAnalysis;

namespace BeamOs.StructuralAnalysis.Contracts.Common;

public readonly record struct LengthContract
{
    public required double Value { get; init; }
    public required LengthUnitContract Unit { get; init; }

    public LengthContract() { }

    [SetsRequiredMembers]
    public LengthContract(double value, LengthUnitContract unit)
    {
        Value = value;
        Unit = unit;
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
        Value = value;
        Unit = unit;
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
        Value = value;
        Unit = unit;
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
        Value = value;
        Unit = unit;
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
        Value = value;
        Unit = unit;
    }
}

public readonly record struct AngleContract
{
    public required double Value { get; init; }
    public required AngleUnitContract Unit { get; init; }

    [SetsRequiredMembers]
    public AngleContract()
        : this(0, AngleUnitContract.Radian) { }

    [SetsRequiredMembers]
    public AngleContract(double value, AngleUnitContract unit)
    {
        Value = value;
        Unit = unit;
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
        Value = value;
        Unit = unit;
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
        Value = value;
        Unit = unit;
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
        Value = value;
        Unit = unit;
    }
}
