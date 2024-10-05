using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.AnalyticalModel;

public record DisplacementsResponse(
    LengthContract DisplacementAlongX,
    LengthContract DisplacementAlongY,
    LengthContract DisplacementAlongZ,
    AngleContract RotationAboutX,
    AngleContract RotationAboutY,
    AngleContract RotationAboutZ
);
