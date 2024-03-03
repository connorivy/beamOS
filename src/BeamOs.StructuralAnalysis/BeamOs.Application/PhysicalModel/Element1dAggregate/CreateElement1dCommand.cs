using BeamOs.Application.Common.Commands;
using UnitsNet;

namespace BeamOs.Application.PhysicalModel.Element1dAggregate;

public record CreateElement1dCommand(
    GuidBasedIdCommand ModelId,
    GuidBasedIdCommand StartNodeId,
    GuidBasedIdCommand EndNodeId,
    GuidBasedIdCommand MaterialId,
    GuidBasedIdCommand SectionProfileId,
    Angle? SectionProfileRotation = null
);
