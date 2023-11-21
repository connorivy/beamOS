using BeamOS.Common.Contracts;

namespace BeamOS.PhysicalModel.Contracts.SectionProfile;
public record SectionProfileResponse(
    UnitValueDTO Area,
    UnitValueDTO StrongAxisMomentOfInertia,
    UnitValueDTO WeakAxisMomentOfInertia,
    UnitValueDTO PolarMomentOfInertia);
