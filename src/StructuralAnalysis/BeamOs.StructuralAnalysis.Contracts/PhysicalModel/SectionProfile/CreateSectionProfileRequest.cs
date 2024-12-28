using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;

public record CreateSectionProfileRequest(
    AreaContract Area,
    AreaMomentOfInertiaContract StrongAxisMomentOfInertia,
    AreaMomentOfInertiaContract WeakAxisMomentOfInertia,
    AreaMomentOfInertiaContract PolarMomentOfInertia,
    AreaContract StrongAxisShearArea,
    AreaContract WeakAxisShearArea
);
