using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Application.AnalyticalResults.ResultSets;

public interface IResultSetRepository : IModelResourceRepository<ResultSetId, ResultSet>
{
    public Task<int> DeleteAll(ModelId modelId, CancellationToken ct);
}
