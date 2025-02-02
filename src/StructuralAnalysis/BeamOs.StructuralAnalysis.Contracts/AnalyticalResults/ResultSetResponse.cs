using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.NodeResult;

namespace BeamOs.StructuralAnalysis.Contracts.AnalyticalResults;

public record ResultSetResponse(int Id, Guid ModelId, List<NodeResultResponse>? NodeResults = null)
    : IModelEntity;
