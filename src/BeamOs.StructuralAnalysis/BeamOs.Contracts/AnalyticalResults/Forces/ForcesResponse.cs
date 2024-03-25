using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.AnalyticalResults.Forces;

public record ForcesResponse(
    UnitValueDto ForceAlongX,
    UnitValueDto ForceAlongY,
    UnitValueDto ForceAlongZ,
    UnitValueDto MomentAboutX,
    UnitValueDto MomentAboutY,
    UnitValueDto MomentAboutZ
);
