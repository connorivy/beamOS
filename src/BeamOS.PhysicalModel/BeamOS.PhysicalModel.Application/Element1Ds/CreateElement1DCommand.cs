using BeamOS.Common.Application.Commands;

namespace BeamOS.PhysicalModel.Application.Element1Ds;

public record CreateElement1DCommand(
    GuidBasedIdCommand ModelId,
    GuidBasedIdCommand StartNodeId,
    GuidBasedIdCommand EndNodeId,
    GuidBasedIdCommand MaterialId,
    GuidBasedIdCommand SectionProfileId);
