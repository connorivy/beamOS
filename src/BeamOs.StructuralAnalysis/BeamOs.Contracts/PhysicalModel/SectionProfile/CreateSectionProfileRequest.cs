using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.PhysicalModel.SectionProfile;

public record CreateSectionProfileRequest(
    string ModelId,
    AreaContract Area,
    AreaMomentOfInertiaContract StrongAxisMomentOfInertia,
    AreaMomentOfInertiaContract WeakAxisMomentOfInertia,
    AreaMomentOfInertiaContract PolarMomentOfInertia,
    AreaContract StrongAxisShearArea,
    AreaContract WeakAxisShearArea
);
