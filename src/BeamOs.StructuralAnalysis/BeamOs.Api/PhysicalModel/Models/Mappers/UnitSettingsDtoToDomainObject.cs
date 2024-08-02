using BeamOs.Api.Common.Mappers;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.Common;
using BeamOs.Domain.Common.ValueObjects;
using UnitsNet.Units;

namespace BeamOs.Api.PhysicalModel.Models.Mappers;

public partial class UnitSettingsDtoToDomainObject : IMapper<UnitSettingsDto, UnitSettings>
{
    public UnitSettings Map(UnitSettingsDto from)
    {
        LengthUnit lengthUnit = UnitsNetMappers.MapToLengthUnit(from.LengthUnit);
        ForceUnit forceUnit = UnitsNetMappers.MapToForceUnit(from.ForceUnit);

        return UnitSettings.Create(lengthUnit, forceUnit);
    }
}
