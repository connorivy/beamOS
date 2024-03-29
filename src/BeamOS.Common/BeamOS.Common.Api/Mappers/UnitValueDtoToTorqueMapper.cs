using BeamOS.Common.Contracts;
using Riok.Mapperly.Abstractions;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.Common.Api.Mappers;

[Mapper]
public static partial class StringToTorqueUnitMapper
{
    public static partial TorqueUnit MapToTorqueUnit(this string unit);
}

public static class UnitValueDtoToTorqueMapper
{
    public static Torque MapToTorque(this UnitValueDTO dto)
    {
        return new(dto.Value, dto.Unit.MapToTorqueUnit());
    }
}
