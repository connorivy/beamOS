using BeamOs.Contracts.Common;
using Riok.Mapperly.Abstractions;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Api.Common.Mappers;

[Mapper]
public static partial class StringToForceUnitMapper
{
    public static partial ForceUnit MapToForceUnit(this string unit);
}

[Mapper]
public static partial class ForceUnitToStringMapper
{
    public static partial string MapToString(this ForceUnit unit);
}

public static class UnitValueDtoToForceMapper
{
    public static Force MapToForce(this UnitValueDto dto)
    {
        return new(dto.Value, dto.Unit.MapToForceUnit());
    }

    public static UnitValueDto ToDto(this Force value, ForceUnit unit)
    {
        return new(value.As(unit), unit.MapToString());
    }
}
