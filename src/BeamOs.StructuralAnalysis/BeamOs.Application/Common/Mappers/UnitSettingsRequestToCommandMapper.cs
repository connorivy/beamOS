using BeamOs.Application.Common.Commands;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Contracts.Common;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.Common.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
public partial class UnitSettingsRequestToCommandMapper
    : IMapper<UnitSettingsDtoVerbose, UnitSettingsCommand>
{
    public UnitSettingsCommand Map(UnitSettingsDtoVerbose from) => this.ToResponse(from);

    public partial UnitSettingsCommand ToResponse(UnitSettingsDtoVerbose model);
}
