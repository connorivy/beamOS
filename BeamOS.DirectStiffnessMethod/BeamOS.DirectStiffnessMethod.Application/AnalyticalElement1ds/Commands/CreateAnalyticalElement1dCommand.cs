using BeamOS.Common.Application.Commands;
using UnitsNet;

namespace BeamOS.DirectStiffnessMethod.Application.AnalyticalElement1ds.Commands;
public record CreateAnalyticalElement1dCommand(
    Angle SectionProfileRotation,
    GuidBasedIdCommand StartNodeId,
    GuidBasedIdCommand EndNodeId,
    string MaterialId,
    string SectionProfileId
    );
