using BeamOs.Application.Common.Commands;
using UnitsNet;

namespace BeamOs.Application.DirectStiffnessMethod.AnalyticalElement1ds.Commands;

public record CreateAnalyticalElement1dCommand(
    Angle SectionProfileRotation,
    GuidBasedIdCommand StartNodeId,
    GuidBasedIdCommand EndNodeId,
    string MaterialId,
    string SectionProfileId
);
