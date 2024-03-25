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

public static class UnitValueDtoToForcePerLengthMapper
{
    public static ForcePerLength MapToForcePerLength(this UnitValueDto dto)
    {
        return new(dto.Value, dto.Unit.MapToForcePerLengthUnit());
    }
}
