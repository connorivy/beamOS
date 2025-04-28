using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Application.AnalyticalResults.ResultSets;

public class DeleteResultSetsCommandHandler(IResultSetRepository resultSetRepository)
    : ICommandHandler<ModelId, int>
{
    public async Task<Result<int>> ExecuteAsync(ModelId query, CancellationToken ct = default) =>
        await resultSetRepository.DeleteAll(query, ct);
}
