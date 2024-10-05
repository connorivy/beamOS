using BeamOs.Contracts.Common;
using Riok.Mapperly.Abstractions;
using UnitsNet.Units;

namespace BeamOs.Application.Common.Mappers.UnitValueDtoMappers;

[Mapper(EnumMappingStrategy = EnumMappingStrategy.ByName)]
[UseStaticMapper(typeof(UnitsNetMappers))]
public static partial class UnitsNetMappersJustEnums
{
    public static partial AngleUnit MapEnumToAngleUnit(this string unit);

    public static partial AngleUnit MapEnumToAngleUnit(this AngleUnitContract unit);

    public static partial string MapEnumToString(this AngleUnit unit);

    public static partial AngleUnitContract MapEnumToContract(this AngleUnit unit);

    public static partial AreaUnit MapEnumToAreaUnit(this string unit);

    public static partial AreaUnit MapEnumToAreaUnit(this AreaUnitContract unit);

    public static partial string MapEnumToString(this AreaUnit unit);

    public static partial AreaUnitContract MapEnumToContract(this AreaUnit unit);

    public static partial AreaMomentOfInertiaUnit MapEnumToAreaMomentOfInertiaUnit(
        this string unit
    );

    public static partial AreaMomentOfInertiaUnit MapEnumToAreaMomentOfInertiaUnit(
        this AreaMomentOfInertiaUnitContract unit
    );

    public static partial string MapEnumToString(this AreaMomentOfInertiaUnit unit);

    public static partial AreaMomentOfInertiaUnitContract MapEnumToContract(
        this AreaMomentOfInertiaUnit unit
    );

    public static partial ForceUnit MapEnumToForceUnit(this string unit);

    public static partial ForceUnit MapEnumToForceUnit(this ForceUnitContract unit);

    public static partial string MapEnumToString(this ForceUnit unit);

    public static partial ForceUnitContract MapEnumToContract(this ForceUnit unit);

    public static partial ForcePerLengthUnit MapEnumToForcePerLengthUnit(this string unit);

    public static partial ForcePerLengthUnit MapEnumToForcePerLengthUnit(
        this ForcePerLengthUnitContract unit
    );

    public static partial string MapEnumToString(this ForcePerLengthUnit unit);

    public static partial ForcePerLengthUnitContract MapEnumToContract(
        this ForcePerLengthUnit unit
    );

    public static partial LengthUnit MapEnumToLengthUnit(this string unit);

    public static partial LengthUnit MapEnumToLengthUnit(this LengthUnitContract unit);

    public static partial string MapEnumToString(this LengthUnit unit);

    public static partial LengthUnitContract MapEnumToContract(this LengthUnit unit);

    public static partial PressureUnit MapEnumToPressureUnit(this string unit);

    public static partial PressureUnit MapEnumToPressureUnit(this PressureUnitContract unit);

    public static partial string MapEnumToString(this PressureUnit unit);

    public static partial PressureUnitContract MapEnumToContract(this PressureUnit unit);

    public static partial TorqueUnit MapEnumToTorqueUnit(this string unit);

    public static partial TorqueUnit MapEnumToTorqueUnit(this TorqueUnitContract unit);

    public static partial string MapEnumToString(this TorqueUnit unit);

    public static partial TorqueUnitContract MapEnumToContract(this TorqueUnit unit);

    public static partial VolumeUnit MapEnumToVolumeUnit(this string unit);

    public static partial VolumeUnit MapEnumToVolumeUnit(this VolumeUnitContract unit);

    public static partial string MapEnumToString(this VolumeUnit unit);

    public static partial VolumeUnitContract MapEnumToContract(this VolumeUnit unit);
}
