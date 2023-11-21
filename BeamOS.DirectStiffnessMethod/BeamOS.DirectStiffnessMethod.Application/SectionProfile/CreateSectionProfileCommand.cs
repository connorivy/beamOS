using UnitsNet;

namespace BeamOS.DirectStiffnessMethod.Application.SectionProfile;
public record CreateSectionProfileCommand(
    Area Area,
    AreaMomentOfInertia StrongAxisMomentOfInertia,
    AreaMomentOfInertia WeakAxisMomentOfInertia,
    AreaMomentOfInertia PolarMomentOfInertia);
