using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.PhysicalModel.SectionProfile;

public record SectionProfileResponse(
    string Id,
    string ModelId,
    AreaContract Area,
    AreaMomentOfInertiaContract StrongAxisMomentOfInertia,
    AreaMomentOfInertiaContract WeakAxisMomentOfInertia,
    AreaMomentOfInertiaContract PolarMomentOfInertia,
    AreaContract StrongAxisShearArea,
    AreaContract WeakAxisShearArea
);
