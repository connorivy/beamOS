using BeamOs.Api.Common.Mappers;
using BeamOS.Common.Contracts;
using Riok.Mapperly.Abstractions;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Api.Common.Mappers;

[Mapper]
public static partial class StringToPressureUnitMapper
{
    public static partial PressureUnit MapToPressureUnit(this string unit);
}

public static class UnitValueDtoToPressureMapper
{
    public static Pressure MapToForce(this UnitValueDTO dto)
    {
        return new(dto.Value, dto.Unit.MapToPressureUnit());
    }
}
