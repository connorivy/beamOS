using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.NodeResult;

public record NodeResultResponse(
    Guid ModelId,
    int ResultSetId,
    int NodeId,
    ForcesResponse Forces,
    DisplacementsResponse Displacements
) : IHasModelId;

public record ForcesResponse(
    ForceContract ForceAlongX,
    ForceContract ForceAlongY,
    ForceContract ForceAlongZ,
    TorqueContract MomentAboutX,
    TorqueContract MomentAboutY,
    TorqueContract MomentAboutZ
);

public record DisplacementsResponse(
    LengthContract DisplacementAlongX,
    LengthContract DisplacementAlongY,
    LengthContract DisplacementAlongZ,
    AngleContract RotationAboutX,
    AngleContract RotationAboutY,
    AngleContract RotationAboutZ
);

public record Element1dResultResponse(
    Guid ModelId,
    int ResultSetId,
    int Element1dId,
    LengthContract MinShear,
    LengthContract MaxShear,
    TorqueContract MinMoment,
    TorqueContract MaxMoment,
    LengthContract MinDisplacement,
    LengthContract MaxDisplacement
) : IHasModelId;
