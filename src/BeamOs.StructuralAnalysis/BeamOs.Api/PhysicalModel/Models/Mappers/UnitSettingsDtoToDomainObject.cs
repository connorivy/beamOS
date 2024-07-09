using BeamOs.Api.Common.Mappers;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.Common;
using BeamOs.Domain.Common.ValueObjects;
using UnitsNet.Units;

namespace BeamOs.Api.PhysicalModel.Models.Mappers;

public partial class UnitSettingsDtoToDomainObject : IMapper<UnitSettingsDto, UnitSettings>
{
    public UnitSettings Map(UnitSettingsDto from)
    {
        LengthUnit lengthUnit = StringToLengthUnitMapper.MapToLengthUnit(from.LengthUnit);
        ForceUnit forceUnit = StringToForceUnitMapper.MapToForceUnit(from.ForceUnit);

        return UnitSettings.Create(lengthUnit, forceUnit);
    }
}
