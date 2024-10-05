using BeamOs.Contracts.AnalyticalModel.Forces;
using BeamOs.Contracts.PhysicalModel.Common;

namespace BeamOs.Contracts.AnalyticalModel.AnalyticalNode;

public record NodeResultResponse(
    string NodeId,
    ForcesResponse Forces,
    DisplacementsResponse Displacements
) : BeamOsContractBase;
