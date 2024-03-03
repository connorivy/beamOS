using BeamOs.Api.Common.Mappers;
using BeamOS.Common.Contracts;
using Riok.Mapperly.Abstractions;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Api.Common.Mappers;

[Mapper]
public static partial class StringToAngleUnitMapper
{
    public static partial AngleUnit MapToAngleUnit(this string unit);
}

public static class UnitValueDtoToAngleMapper
{
    public static Angle MapToAngle(this UnitValueDTO dto)
    {
        return new(dto.Value, dto.Unit.MapToAngleUnit());
    }
}
