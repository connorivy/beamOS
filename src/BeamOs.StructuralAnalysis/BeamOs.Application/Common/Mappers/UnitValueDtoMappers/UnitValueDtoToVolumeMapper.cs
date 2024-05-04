using BeamOs.Contracts.Common;
using Riok.Mapperly.Abstractions;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Api.Common.Mappers.UnitValueDtoMappers;

[Mapper]
public static partial class StringToVolumeUnitMapper
{
    public static partial VolumeUnit MapToVolumeUnit(this string unit);
}

[Mapper]
public static partial class VolumeUnitToStringMapper
{
    public static partial string MapToString(this VolumeUnit unit);
}

public static class UnitValueDtoToVolumeMapper
{
    public static Volume MapToVolume(this UnitValueDto dto)
    {
        return new(dto.Value, dto.Unit.MapToVolumeUnit());
    }
}
