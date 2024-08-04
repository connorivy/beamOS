using Riok.Mapperly.Abstractions;
using UnitsNet.Units;

namespace BeamOs.Application.Common.Mappers.UnitValueDtoMappers;

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
public static partial class UnitsNetMappersJustEnums
{
    public static partial AngleUnit MapEnumToAngleUnit(this string unit);

    public static partial string MapEnumToString(this AngleUnit unit);

    public static partial AreaUnit MapEnumToAreaUnit(this string unit);

    public static partial string MapEnumToString(this AreaUnit unit);

    public static partial AreaMomentOfInertiaUnit MapEnumToAreaMomentOfInertiaUnit(
        this string unit
    );

    public static partial string MapEnumToString(this AreaMomentOfInertiaUnit unit);

    public static partial ForceUnit MapEnumToForceUnit(this string unit);

    public static partial string MapEnumToString(this ForceUnit unit);

    public static partial ForcePerLengthUnit MapEnumToForcePerLengthUnit(this string unit);

    public static partial string MapEnumToString(this ForcePerLengthUnit unit);

    public static partial LengthUnit MapEnumToLengthUnit(this string unit);

    public static partial string MapEnumToString(this LengthUnit unit);

    public static partial PressureUnit MapEnumToPressureUnit(this string unit);

    public static partial string MapEnumToString(this PressureUnit unit);

    public static partial TorqueUnit MapEnumToTorqueUnit(this string unit);

    public static partial string MapEnumToString(this TorqueUnit unit);

    public static partial VolumeUnit MapEnumToVolumeUnit(this string unit);

    public static partial string MapEnumToString(this VolumeUnit unit);
}
