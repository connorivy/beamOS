using Riok.Mapperly.Abstractions;
using UnitsNet.Units;

namespace BeamOs.Application.Common.Mappers.UnitValueDtoMappers;

[Mapper]
public static partial class UnitsNetEnumMapper
{
    public static partial AngleUnit MapToAngleUnit(this string unit);

    public static partial string MapToString(this AngleUnit unit);

    public static partial AreaUnit MapToAreaUnit(this string unit);

    public static partial string MapToString(this AreaUnit unit);

    public static partial AreaMomentOfInertiaUnit MapToAreaMomentOfInertiaUnit(this string unit);

    public static partial string MapToString(this AreaMomentOfInertiaUnit unit);

    public static partial ForceUnit MapToForceUnit(this string unit);

    public static partial string MapToString(this ForceUnit unit);

    public static partial ForcePerLengthUnit MapToForcePerLengthUnit(this string unit);

    public static partial string MapToString(this ForcePerLengthUnit unit);

    public static partial LengthUnit MapToLengthUnit(this string unit);

    public static partial string MapToString(this LengthUnit unit);

    public static partial PressureUnit MapToPressureUnit(this string unit);

    public static partial string MapToString(this PressureUnit unit);

    public static partial TorqueUnit MapToTorqueUnit(this string unit);

    public static partial string MapToString(this TorqueUnit unit);

    public static partial VolumeUnit MapToVolumeUnit(this string unit);

    public static partial string MapToString(this VolumeUnit unit);
}
