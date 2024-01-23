using BeamOS.Common.Application.Commands;
using UnitsNet;

namespace BeamOS.PhysicalModel.Application.SectionProfiles;

public record CreateSectionProfileCommand(
    GuidBasedIdCommand ModelId,
    Area Area,
    AreaMomentOfInertia StrongAxisMomentOfInertia,
    AreaMomentOfInertia WeakAxisMomentOfInertia,
    AreaMomentOfInertia PolarMomentOfInertia
);
