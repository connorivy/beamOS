using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.AnalyticalResults;

public record DisplacementsResponse(
    UnitValueDto DisplacementAlongX,
    UnitValueDto DisplacementAlongY,
    UnitValueDto DisplacementAlongZ,
    UnitValueDto RotationAboutX,
    UnitValueDto RotationAboutY,
    UnitValueDto RotationAboutZ
);
