using BeamOS.Common.Contracts;

namespace BeamOS.PhysicalModel.Contracts.SectionProfile;

public record CreateSectionProfileRequest(
    string ModelId,
    UnitValueDTO Area,
    UnitValueDTO StrongAxisMomentOfInertia,
    UnitValueDTO WeakAxisMomentOfInertia,
    UnitValueDTO PolarMomentOfInertia
);
