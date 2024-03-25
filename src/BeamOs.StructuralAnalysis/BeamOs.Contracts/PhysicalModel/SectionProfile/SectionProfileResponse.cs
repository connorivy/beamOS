using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.PhysicalModel.SectionProfile;

public record SectionProfileResponse(
    string Id,
    UnitValueDto Area,
    UnitValueDto StrongAxisMomentOfInertia,
    UnitValueDto WeakAxisMomentOfInertia,
    UnitValueDto PolarMomentOfInertia
);
