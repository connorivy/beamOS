using BeamOs.Contracts.Common;
using Riok.Mapperly.Abstractions;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Api.Common.Mappers;

[Mapper]
public static partial class StringToAreaMomentOfInertiaUnitMapper
{
    public static partial AreaMomentOfInertiaUnit MapToAreaMomentOfInertiaUnit(this string unit);
}

public static class UnitValueDtoToAreaMomentOfInertiaMapper
{
    public static AreaMomentOfInertia MapToAreaMomentOfInertia(this UnitValueDto dto)
    {
        return new(dto.Value, dto.Unit.MapToAreaMomentOfInertiaUnit());
    }
}
