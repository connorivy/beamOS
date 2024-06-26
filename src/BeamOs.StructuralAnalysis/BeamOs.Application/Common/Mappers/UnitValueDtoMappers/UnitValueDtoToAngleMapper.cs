using BeamOs.Contracts.Common;
using Riok.Mapperly.Abstractions;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Api.Common.Mappers;

[Mapper]
public static partial class StringToAngleUnitMapper
{
    public static partial AngleUnit MapToAngleUnit(this string unit);
}

[Mapper]
public static partial class AngleUnitToStringMapper
{
    public static partial string MapToString(this AngleUnit unit);
}

public static class UnitValueDtoToAngleMapper
{
    public static Angle MapToAngle(this UnitValueDto dto)
    {
        return new(dto.Value, dto.Unit.MapToAngleUnit());
    }

    public static UnitValueDto ToDto(this Angle value, AngleUnit? unitOverride = null)
    {
        if (unitOverride is not null)
        {
            return new(value.As(unitOverride.Value), unitOverride.Value.MapToString());
        }
        return new(value.Value, value.Unit.MapToString());
    }
}
