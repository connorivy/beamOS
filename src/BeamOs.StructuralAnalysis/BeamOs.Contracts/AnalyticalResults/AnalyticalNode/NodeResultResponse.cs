using BeamOs.Contracts.AnalyticalResults.Forces;
using BeamOs.Contracts.PhysicalModel.Common;

namespace BeamOs.Contracts.AnalyticalResults.AnalyticalNode;

public record NodeResultResponse(
    string NodeId,
    ForcesResponse Forces,
    DisplacementsResponse Displacements
) : BeamOsContractBase;
