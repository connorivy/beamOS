using BeamOs.Contracts.Common;
using Riok.Mapperly.Abstractions;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Api.Common.Mappers;

[Mapper]
public static partial class StringToLengthUnitMapper
{
    public static partial LengthUnit MapToLengthUnit(this string unit);
}

public static class UnitValueDtoToLengthMapper
{
    public static Length MapToLength(this UnitValueDto dto)
    {
        return new(dto.Value, dto.Unit.MapToLengthUnit());
    }
}

[Mapper]
public static partial class LengthUnitToStringMapper
{
    public static partial string MapToString(this LengthUnit unit);
}

public static class LengthToUnitValueDtoMapperMapper
{
    public static UnitValueDto ToDto(this Length value, LengthUnit unit)
    {
        return new(value.As(unit), unit.MapToString());
    }
}
