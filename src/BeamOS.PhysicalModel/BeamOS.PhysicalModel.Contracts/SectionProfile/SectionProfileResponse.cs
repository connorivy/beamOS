using BeamOS.Common.Contracts;

namespace BeamOS.PhysicalModel.Contracts.SectionProfile;

public record SectionProfileResponse(
    string Id,
    UnitValueDTO Area,
    UnitValueDTO StrongAxisMomentOfInertia,
    UnitValueDTO WeakAxisMomentOfInertia,
    UnitValueDTO PolarMomentOfInertia
);
