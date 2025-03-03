using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Domain.OpenSees;

public class BeamOsToOpenSeesConverter(
    ModelSettings modelSettings,
    UnitSettings? unitSettingsOverride = null
)
{
    protected UnitSettings UnitSettings => unitSettingsOverride ?? modelSettings.UnitSettings;

    public string Element1d(
        Element1d element1d,
        Material material,
        SectionProfile sectionProfile,
        int transformId
    )
    {
        if (modelSettings.AnalysisSettings.Element1DAnalysisType == Element1dAnalysisType.Euler)
        {
            return OpenSeesNetModel.ElasticBeamColumn(
                element1d.Id,
                element1d.StartNodeId,
                element1d.EndNodeId,
                sectionProfile.Area.As(this.UnitSettings.AreaUnit),
                material.ModulusOfElasticity.As(this.UnitSettings.PressureUnit),
                material.ModulusOfRigidity.As(this.UnitSettings.PressureUnit),
                sectionProfile
                    .StrongAxisMomentOfInertia
                    .As(this.UnitSettings.AreaMomentOfInertiaUnit),
                sectionProfile
                    .WeakAxisMomentOfInertia
                    .As(this.UnitSettings.AreaMomentOfInertiaUnit),
                sectionProfile.PolarMomentOfInertia.As(this.UnitSettings.AreaMomentOfInertiaUnit),
                transformId
            );
        }
        else
        {
            return OpenSeesNetModel.ElasticTimoshenkoBeamColumn(
                element1d.Id,
                element1d.StartNodeId,
                element1d.EndNodeId,
                sectionProfile.Area.As(this.UnitSettings.AreaUnit),
                material.ModulusOfElasticity.As(this.UnitSettings.PressureUnit),
                material.ModulusOfRigidity.As(this.UnitSettings.PressureUnit),
                sectionProfile
                    .StrongAxisMomentOfInertia
                    .As(this.UnitSettings.AreaMomentOfInertiaUnit),
                sectionProfile
                    .WeakAxisMomentOfInertia
                    .As(this.UnitSettings.AreaMomentOfInertiaUnit),
                sectionProfile.PolarMomentOfInertia.As(this.UnitSettings.AreaMomentOfInertiaUnit),
                sectionProfile.StrongAxisShearArea.As(this.UnitSettings.AreaUnit),
                sectionProfile.WeakAxisShearArea.As(this.UnitSettings.AreaUnit),
                transformId
            );
        }
    }
}
