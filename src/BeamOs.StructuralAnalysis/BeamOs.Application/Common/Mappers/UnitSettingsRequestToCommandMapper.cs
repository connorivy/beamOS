using BeamOs.Api.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Application.Common.Commands;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.Common;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.Common.Mappers;

[Mapper]
[UseStaticMapper(typeof(StringToLengthUnitMapper))]
[UseStaticMapper(typeof(StringToAreaUnitMapper))]
[UseStaticMapper(typeof(StringToVolumeUnitMapper))]
[UseStaticMapper(typeof(StringToForceUnitMapper))]
[UseStaticMapper(typeof(StringToForcePerLengthUnitMapper))]
[UseStaticMapper(typeof(StringToTorqueUnitMapper))]
[UseStaticMapper(typeof(StringToPressureUnitMapper))]
[UseStaticMapper(typeof(StringToAreaMomentOfInertiaUnitMapper))]
public partial class UnitSettingsRequestToCommandMapper
    : IMapper<UnitSettingsDtoVerbose, UnitSettingsCommand>
{
    public UnitSettingsCommand Map(UnitSettingsDtoVerbose from) => this.ToResponse(from);

    public partial UnitSettingsCommand ToResponse(UnitSettingsDtoVerbose model);
}
