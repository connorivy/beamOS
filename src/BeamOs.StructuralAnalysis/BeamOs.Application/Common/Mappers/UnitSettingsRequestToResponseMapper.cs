using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Domain.Common.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.Common.Mappers;

[Mapper]
public static partial class UnitSettingsRequestToResponseMapper
{
    public static partial UnitSettingsResponse ToResponse(this UnitSettings model);

    public static partial UnitSettingsDtoVerbose ToContract(this UnitSettings model);
}
