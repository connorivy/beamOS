using BeamOs.Contracts.Common;
using Riok.Mapperly.Abstractions;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Api.Common.Mappers;

[Mapper]
public static partial class StringToTorqueUnitMapper
{
    public static partial TorqueUnit MapToTorqueUnit(this string unit);
}

public static class UnitValueDtoToTorqueMapper
{
    public static Torque MapToTorque(this UnitValueDto dto)
    {
        return new(dto.Value, dto.Unit.MapToTorqueUnit());
    }
}
