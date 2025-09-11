using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Application.AnalyticalResults.ResultSets;

internal interface IResultSetRepository : IModelResourceRepository<ResultSetId, ResultSet>
{
    public Task<int> DeleteAll(ModelId modelId, CancellationToken ct);
    public Task<ResultSet?> GetSingle(
        ModelId modelId,
        ResultSetId resultSetId,
        CancellationToken ct,
        params string[] resultSetMembersToLoad
    );
}

internal sealed class InMemoryResultSetRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
)
    : InMemoryModelResourceRepository<ResultSetId, ResultSet>(inMemoryModelRepositoryStorage),
        IResultSetRepository
{
    public Task<int> DeleteAll(ModelId modelId, CancellationToken ct)
    {
        int result = 0;
        var modelResultSets = this.ModelResources.GetValueOrDefault(modelId);

        if (modelResultSets is not null)
        {
            result = modelResultSets.Count;
            modelResultSets.Clear();
        }
        return Task.FromResult(result);
    }

    public Task<ResultSet?> GetSingle(
        ModelId modelId,
        ResultSetId resultSetId,
        CancellationToken ct,
        params string[] resultSetMembersToLoad
    )
    {
        if (this.ModelResources.TryGetValue(modelId, out var modelResultSets))
        {
            if (modelResultSets.TryGetValue(resultSetId, out var resultSet))
            {
                return Task.FromResult<ResultSet?>(resultSet);
            }
        }
        return Task.FromResult<ResultSet?>(null);
    }
}
