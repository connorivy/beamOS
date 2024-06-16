using BeamOs.Contracts.Common;
using Riok.Mapperly.Abstractions;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Api.Common.Mappers.UnitValueDtoMappers;

[Mapper]
public static partial class StringToForcePerLengthUnitMapper
{
    public static partial ForcePerLengthUnit MapToForcePerLengthUnit(this string unit);
}

[Mapper]
public static partial class ForcePerLengthUnitToStringMapper
{
    public static partial string MapToString(this ForcePerLengthUnit unit);
}

public static class UnitValueDtoToForcePerLengthMapper
{
    public static ForcePerLength MapToForcePerLength(this UnitValueDto dto)
    {
        return new(dto.Value, dto.Unit.MapToForcePerLengthUnit());
    }

    public static UnitValueDto ToDto(this ForcePerLength value, ForcePerLengthUnit unit)
    {
        return new(value.As(unit), unit.MapToString());
    }
}
