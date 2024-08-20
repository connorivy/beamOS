using BeamOs.Application.AnalyticalResults.ModelResults;
using BeamOs.Domain.AnalyticalResults.ModelResultAggregate;
using BeamOs.Domain.AnalyticalResults.ModelResultAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.Repositories.AnalyticalResults.ModelResults;

internal class ModelResultDbContextRepository(BeamOsStructuralDbContext dbContext)
    : RepositoryBase<ModelResultId, ModelResult>(dbContext),
        IModelResultRepository
{
    public async Task<ModelResult?> GetByModelId(ModelId modelId, CancellationToken ct = default) =>
        await this.DbContext
            .ModelResults
            .FirstOrDefaultAsync(el => el.ModelId == modelId, cancellationToken: ct);
}
