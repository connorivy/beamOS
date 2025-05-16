using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.NodeResult;

namespace BeamOs.StructuralAnalysis.Contracts.AnalyticalResults;

public record ResultSetResponse(
    int Id,
    Guid ModelId,
    List<NodeResultResponse>? NodeResults = null,
    List<Element1dResultResponse>? Element1dResults = null
) : IModelEntity;

public record ResultSet(
    int Id,
    Guid ModelId,
    List<NodeResultResponse>? NodeResults = null,
    List<Element1dResultResponse>? Element1dResults = null
) : IModelEntity;
