using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.PhysicalModel.SectionProfile;

public record SectionProfileResponse(
    string Id,
    string ModelId,
    UnitValueDto Area,
    UnitValueDto StrongAxisMomentOfInertia,
    UnitValueDto WeakAxisMomentOfInertia,
    UnitValueDto PolarMomentOfInertia,
    UnitValueDto StrongAxisShearArea,
    UnitValueDto WeakAxisShearArea
);
