using BeamOS.Common.Contracts;
using Riok.Mapperly.Abstractions;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.Common.Api.Mappers;

[Mapper]
public static partial class StringToForceUnitMapper
{
    public static partial ForceUnit MapToForceUnit(this string unit);
}
public static class UnitValueDtoToForceMapper
{
    public static Force MapToForce(this UnitValueDTO dto)
    {
        return new(dto.Value, dto.Unit.MapToForceUnit());
    }
}
