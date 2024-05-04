using BeamOs.Contracts.Common;
using Riok.Mapperly.Abstractions;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Api.Common.Mappers;

[Mapper]
public static partial class StringToPressureUnitMapper
{
    public static partial PressureUnit MapToPressureUnit(this string unit);
}

[Mapper]
public static partial class PressureUnitToStringMapper
{
    public static partial string MapToString(this PressureUnit unit);
}

public static class UnitValueDtoToPressureMapper
{
    public static Pressure MapToForce(this UnitValueDto dto)
    {
        return new(dto.Value, dto.Unit.MapToPressureUnit());
    }
}
