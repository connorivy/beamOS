using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;

public record SectionProfileResponse(
    int Id,
    Guid ModelId,
    double Area,
    double StrongAxisMomentOfInertia,
    double WeakAxisMomentOfInertia,
    double PolarMomentOfInertia,
    double? StrongAxisShearArea,
    double? WeakAxisShearArea,
    AreaUnit AreaUnit,
    AreaMomentOfInertiaUnit AreaMomentOfInertiaUnit
) : IModelEntity
{
    public SectionProfileData ToSectionProfileData() =>
        new()
        {
            Area = this.Area,
            StrongAxisMomentOfInertia = this.StrongAxisMomentOfInertia,
            WeakAxisMomentOfInertia = this.WeakAxisMomentOfInertia,
            PolarMomentOfInertia = this.PolarMomentOfInertia,
            StrongAxisShearArea = this.StrongAxisShearArea,
            WeakAxisShearArea = this.WeakAxisShearArea,
            AreaUnit = this.AreaUnit,
            AreaMomentOfInertiaUnit = this.AreaMomentOfInertiaUnit,
        };
}
