using BeamOs.Api.Common.Mappers;
using BeamOS.Common.Contracts;
using Riok.Mapperly.Abstractions;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Api.Common.Mappers;

[Mapper]
public static partial class StringToAreaUnitMapper
{
    public static partial AreaUnit MapToAreaUnit(this string unit);
}

public static class UnitValueDtoToAreaMapper
{
    public static Area MapToArea(this UnitValueDTO dto)
    {
        return new(dto.Value, dto.Unit.MapToAreaUnit());
    }
}
