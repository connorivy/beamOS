using UnitsNet.Units;

namespace BeamOs.Infrastructure.Data.Interfaces;

internal interface IFlattenedModelSettings
{
    public AngleUnit Settings_UnitSettings_AngleUnit { get; }
    public AreaMomentOfInertiaUnit Settings_UnitSettings_AreaMomentOfInertiaUnit { get; }
    public AreaUnit Settings_UnitSettings_AreaUnit { get; }
    public ForcePerLengthUnit Settings_UnitSettings_ForcePerLengthUnit { get; }
    public ForceUnit Settings_UnitSettings_ForceUnit { get; }
    public LengthUnit Settings_UnitSettings_LengthUnit { get; }
    public PressureUnit Settings_UnitSettings_PressureUnit { get; }
    public TorqueUnit Settings_UnitSettings_TorqueUnit { get; }
    public VolumeUnit Settings_UnitSettings_VolumeUnit { get; }
}
