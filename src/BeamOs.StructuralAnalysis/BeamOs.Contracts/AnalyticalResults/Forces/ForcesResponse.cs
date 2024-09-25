using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.AnalyticalResults.Forces;

public record ForcesResponse(
    ForceContract ForceAlongX,
    ForceContract ForceAlongY,
    ForceContract ForceAlongZ,
    TorqueContract MomentAboutX,
    TorqueContract MomentAboutY,
    TorqueContract MomentAboutZ
);
