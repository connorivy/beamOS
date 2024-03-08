using UnitsNet;

namespace BeamOs.Application.DirectStiffnessMethod.SectionProfiles;

public record CreateSectionProfileCommand(
    string Id,
    Area Area,
    AreaMomentOfInertia StrongAxisMomentOfInertia,
    AreaMomentOfInertia WeakAxisMomentOfInertia,
    AreaMomentOfInertia PolarMomentOfInertia
);
