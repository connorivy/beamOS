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
    Force ForceAlongX,
    Force ForceAlongY,
    Force ForceAlongZ,
    Torque MomentAboutX,
    Torque MomentAboutY,
    Torque MomentAboutZ
);

public record DisplacementsResponse(
    Length DisplacementAlongX,
    Length DisplacementAlongY,
    Length DisplacementAlongZ,
    Angle RotationAboutX,
    Angle RotationAboutY,
    Angle RotationAboutZ
);

public record Element1dResultResponse(
    Guid ModelId,
    int ResultSetId,
    int Element1dId,
    Length MinShear,
    Length MaxShear,
    Torque MinMoment,
    Torque MaxMoment,
    Length MinDisplacement,
    Length MaxDisplacement
) : IHasModelId;
