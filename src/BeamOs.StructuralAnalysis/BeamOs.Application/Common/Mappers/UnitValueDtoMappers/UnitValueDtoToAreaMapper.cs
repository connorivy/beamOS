using BeamOs.Contracts.Common;
using Riok.Mapperly.Abstractions;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Api.Common.Mappers;

[Mapper]
public static partial class StringToAreaUnitMapper
{
    public static partial AreaUnit MapToAreaUnit(this string unit);
}

[Mapper]
public static partial class AreaUnitToStringMapper
{
    public static partial string MapToString(this AreaUnit unit);
}

public static class UnitValueDtoToAreaMapper
{
    public static Area MapToArea(this UnitValueDto dto)
    {
        return new(dto.Value, dto.Unit.MapToAreaUnit());
    }
}
