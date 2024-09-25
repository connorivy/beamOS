using BeamOs.Application.AnalyticalModel.ModelResults;
using BeamOs.Domain.AnalyticalModel.AnalyticalResultsAggregate;
using BeamOs.Domain.AnalyticalModel.AnalyticalResultsAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.Repositories.AnalyticalModel.ModelResults;

internal class ModelResultDbContextRepository(BeamOsStructuralDbContext dbContext)
    : RepositoryBase<AnalyticalResultsId, AnalyticalResults>(dbContext),
        IModelResultRepository
{
    public async Task<AnalyticalResults?> GetByModelId(
        ModelId modelId,
        CancellationToken ct = default
    ) =>
        await this.DbContext
            .ModelResults
            .FirstOrDefaultAsync(el => el.ModelId == modelId, cancellationToken: ct);
}
