using BeamOs.Application.Common.Interfaces;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Infrastructure.Data.Interfaces;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Models.Mappers;

[Mapper]
internal partial class IFlattenedModelSettingsToModelSettingsResponseMapper
    : IMapper<IFlattenedModelSettings, ModelSettingsResponse>
{
    public ModelSettingsResponse Map(IFlattenedModelSettings source)
    {
        return new(this.ToUnitSettingsResponse(source));
    }

    [MapProperty(
        nameof(@IFlattenedModelSettings.Settings_UnitSettings_AreaMomentOfInertiaUnit),
        nameof(@UnitSettingsResponse.AreaMomentOfInertiaUnit)
    )]
    [MapProperty(
        nameof(@IFlattenedModelSettings.Settings_UnitSettings_AreaUnit),
        nameof(@UnitSettingsResponse.AreaUnit)
    )]
    [MapProperty(
        nameof(@IFlattenedModelSettings.Settings_UnitSettings_ForcePerLengthUnit),
        nameof(@UnitSettingsResponse.ForcePerLengthUnit)
    )]
    [MapProperty(
        nameof(@IFlattenedModelSettings.Settings_UnitSettings_ForceUnit),
        nameof(@UnitSettingsResponse.ForceUnit)
    )]
    [MapProperty(
        nameof(@IFlattenedModelSettings.Settings_UnitSettings_LengthUnit),
        nameof(@UnitSettingsResponse.LengthUnit)
    )]
    [MapProperty(
        nameof(@IFlattenedModelSettings.Settings_UnitSettings_PressureUnit),
        nameof(@UnitSettingsResponse.PressureUnit)
    )]
    [MapProperty(
        nameof(@IFlattenedModelSettings.Settings_UnitSettings_TorqueUnit),
        nameof(@UnitSettingsResponse.TorqueUnit)
    )]
    [MapProperty(
        nameof(@IFlattenedModelSettings.Settings_UnitSettings_VolumeUnit),
        nameof(@UnitSettingsResponse.VolumeUnit)
    )]
    public partial UnitSettingsResponse ToUnitSettingsResponse(IFlattenedModelSettings source);
}
