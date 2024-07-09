using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.Common;
using BeamOs.Domain.Common.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.Models.Mappers;

[Mapper]
public partial class UnitSettingsRequestToDomainObject
    : IMapper<UnitSettingsDtoVerbose, UnitSettings>
{
    public UnitSettings Map(UnitSettingsDtoVerbose from) => this.ToResponse(from);

    public partial UnitSettings ToResponse(UnitSettingsDtoVerbose request);
}
