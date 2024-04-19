using BeamOs.Infrastructure.Data.Interfaces;
using UnitsNet.Units;

namespace BeamOs.Infrastructure.Data.Models;

internal sealed class ModelReadModel : ReadModelBase, IFlattenedModelSettings
{
    public string? Name { get; private set; }
    public string? Description { get; private set; }
    public AngleUnit Settings_UnitSettings_AngleUnit { get; private set; }
    public AreaMomentOfInertiaUnit Settings_UnitSettings_AreaMomentOfInertiaUnit
    {
        get;
        private set;
    }
    public AreaUnit Settings_UnitSettings_AreaUnit { get; private set; }
    public ForcePerLengthUnit Settings_UnitSettings_ForcePerLengthUnit { get; private set; }
    public ForceUnit Settings_UnitSettings_ForceUnit { get; private set; }
    public LengthUnit Settings_UnitSettings_LengthUnit { get; private set; }
    public PressureUnit Settings_UnitSettings_PressureUnit { get; private set; }
    public TorqueUnit Settings_UnitSettings_TorqueUnit { get; private set; }
    public VolumeUnit Settings_UnitSettings_VolumeUnit { get; private set; }
    public List<NodeReadModel>? Nodes { get; private set; }
    public List<Element1dReadModel>? Element1ds { get; private set; }
    public List<MaterialReadModel>? Materials { get; private set; }
    public List<SectionProfileReadModel>? SectionProfiles { get; private set; }
    //public List<PointLoadReadModel>? PointLoads { get; private set; }
    //public List<MomentLoadReadModel>? MomentLoads { get; private set; }
}
