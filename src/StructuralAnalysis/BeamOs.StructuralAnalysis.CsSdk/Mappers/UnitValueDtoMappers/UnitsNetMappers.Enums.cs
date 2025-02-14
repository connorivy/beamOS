using BeamOs.StructuralAnalysis.Contracts.Common;
using UnitsNet.Units;

namespace BeamOs.Application.Common.Mappers.UnitValueDtoMappers;

public static partial class UnitsNetMappers
{
    public static partial AngleUnit MapToAngleUnit(this string unit);

    public static partial AngleUnit MapToAngleUnit(this AngleUnitContract unit);

    public static partial AngleUnitContract MapToContract(this AngleUnit unit);

    public static partial AreaUnit MapToAreaUnit(this string unit);

    public static partial AreaUnit MapToAreaUnit(this AreaUnitContract unit);

    public static partial AreaUnitContract MapToContract(this AreaUnit unit);

    public static partial AreaMomentOfInertiaUnit MapToAreaMomentOfInertiaUnit(this string unit);

    public static partial AreaMomentOfInertiaUnit MapToAreaMomentOfInertiaUnit(
        this AreaMomentOfInertiaUnitContract unit
    );

    public static partial string MapToString(this AreaMomentOfInertiaUnit unit);

    public static partial AreaMomentOfInertiaUnitContract MapToContract(
        this AreaMomentOfInertiaUnit unit
    );

    public static partial ForceUnit MapToForceUnit(this string unit);

    public static partial ForceUnit MapToForceUnit(this ForceUnitContract unit);

    public static partial string MapToString(this ForceUnit unit);

    public static partial ForceUnitContract MapToContract(this ForceUnit unit);

    public static partial ForcePerLengthUnit MapToForcePerLengthUnit(this string unit);

    public static partial ForcePerLengthUnit MapToForcePerLengthUnit(
        this ForcePerLengthUnitContract unit
    );

    public static partial string MapToString(this ForcePerLengthUnit unit);

    public static partial ForcePerLengthUnitContract MapToContract(this ForcePerLengthUnit unit);

    public static partial LengthUnit MapToLengthUnit(this string unit);

    public static partial LengthUnit MapToLengthUnit(this LengthUnitContract unit);

    public static partial string MapToString(this LengthUnit unit);

    public static partial LengthUnitContract MapToContract(this LengthUnit unit);

    public static partial PressureUnit MapToPressureUnit(this string unit);

    public static partial PressureUnit MapToPressureUnit(this PressureUnitContract unit);

    public static partial string MapToString(this PressureUnit unit);

    public static partial PressureUnitContract MapToContract(this PressureUnit unit);

    public static partial TorqueUnit MapToTorqueUnit(this string unit);

    public static partial TorqueUnit MapToTorqueUnit(this TorqueUnitContract unit);

    public static partial string MapToString(this TorqueUnit unit);

    public static partial TorqueUnitContract MapToContract(this TorqueUnit unit);

    public static partial VolumeUnit MapToVolumeUnit(this string unit);

    public static partial VolumeUnit MapToVolumeUnit(this VolumeUnitContract unit);

    public static partial string MapToString(this VolumeUnit unit);

    public static partial VolumeUnitContract MapToContract(this VolumeUnit unit);
}
