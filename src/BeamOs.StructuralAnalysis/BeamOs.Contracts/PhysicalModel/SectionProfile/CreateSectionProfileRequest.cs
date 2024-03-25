using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.PhysicalModel.SectionProfile;

public record CreateSectionProfileRequest(
    string ModelId,
    UnitValueDto Area,
    UnitValueDto StrongAxisMomentOfInertia,
    UnitValueDto WeakAxisMomentOfInertia,
    UnitValueDto PolarMomentOfInertia
);
