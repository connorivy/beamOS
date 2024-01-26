using UnitsNet;

namespace BeamOS.DirectStiffnessMethod.Application.SectionProfiles;

public record CreateSectionProfileCommand(
    string Id,
    Area Area,
    AreaMomentOfInertia StrongAxisMomentOfInertia,
    AreaMomentOfInertia WeakAxisMomentOfInertia,
    AreaMomentOfInertia PolarMomentOfInertia
);
