using BeamOs.Application.Common.Interfaces;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Infrastructure.Data.Interfaces;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Models.Mappers;

[Mapper]
internal partial class IFlattenedModelSettingsToUnitSettingsDomainObjectMapper
    : IMapper<IFlattenedModelSettings, UnitSettings>
{
    public UnitSettings Map(IFlattenedModelSettings source) => this.ToResponse(source);

    [MapProperty(
        nameof(@IFlattenedModelSettings.Settings_UnitSettings_AreaMomentOfInertiaUnit),
        nameof(@UnitSettings.AreaMomentOfInertiaUnit)
    )]
    [MapProperty(
        nameof(@IFlattenedModelSettings.Settings_UnitSettings_AreaUnit),
        nameof(@UnitSettings.AreaUnit)
    )]
    [MapProperty(
        nameof(@IFlattenedModelSettings.Settings_UnitSettings_ForcePerLengthUnit),
        nameof(@UnitSettings.ForcePerLengthUnit)
    )]
    [MapProperty(
        nameof(@IFlattenedModelSettings.Settings_UnitSettings_ForceUnit),
        nameof(@UnitSettings.ForceUnit)
    )]
    [MapProperty(
        nameof(@IFlattenedModelSettings.Settings_UnitSettings_LengthUnit),
        nameof(@UnitSettings.LengthUnit)
    )]
    [MapProperty(
        nameof(@IFlattenedModelSettings.Settings_UnitSettings_PressureUnit),
        nameof(@UnitSettings.PressureUnit)
    )]
    [MapProperty(
        nameof(@IFlattenedModelSettings.Settings_UnitSettings_TorqueUnit),
        nameof(@UnitSettings.TorqueUnit)
    )]
    [MapProperty(
        nameof(@IFlattenedModelSettings.Settings_UnitSettings_VolumeUnit),
        nameof(@UnitSettings.VolumeUnit)
    )]
    public partial UnitSettings ToResponse(IFlattenedModelSettings source);
}
