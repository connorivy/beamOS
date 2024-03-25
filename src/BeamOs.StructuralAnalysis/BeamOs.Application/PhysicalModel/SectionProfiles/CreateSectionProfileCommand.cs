using BeamOs.Application.Common.Commands;
using UnitsNet;

namespace BeamOs.Application.PhysicalModel.SectionProfiles;

public record CreateSectionProfileCommand(
    GuidBasedIdCommand ModelId,
    Area Area,
    AreaMomentOfInertia StrongAxisMomentOfInertia,
    AreaMomentOfInertia WeakAxisMomentOfInertia,
    AreaMomentOfInertia PolarMomentOfInertia
);
