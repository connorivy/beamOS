using BeamOS.Common.Contracts;
using Riok.Mapperly.Abstractions;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.Common.Api.Mappers;

[Mapper]
public static partial class StringToAreaMomentOfInertiaUnitMapper
{
    public static partial AreaMomentOfInertiaUnit MapToAreaMomentOfInertiaUnit(this string unit);
}

public static class UnitValueDtoToAreaMomentOfInertiaMapper
{
    public static AreaMomentOfInertia MapToAreaMomentOfInertia(this UnitValueDTO dto)
    {
        return new(dto.Value, dto.Unit.MapToAreaMomentOfInertiaUnit());
    }
}
