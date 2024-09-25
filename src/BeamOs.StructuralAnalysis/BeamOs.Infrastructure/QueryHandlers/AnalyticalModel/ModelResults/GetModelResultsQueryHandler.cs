using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.AnalyticalResults.Model;
using BeamOs.Contracts.Common;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Infrastructure.QueryHandlers.AnalyticalModel.ModelResults.Mappers;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.QueryHandlers.AnalyticalModel.ModelResults;

public sealed class GetModelResultsQueryHandler(BeamOsStructuralDbContext dbContext)
    : IQueryHandler<ModelIdRequest, ModelResultResponse>
{
    public async Task<ModelResultResponse?> ExecuteAsync(
        ModelIdRequest query,
        CancellationToken ct = default
    )
    {
        ModelId modelId = new(Guid.Parse(query.ModelId));
        return await dbContext
            .Models
            .Include(m => m.AnalyticalResults)
            .Where(m => m.Id == modelId)
            .Select(
                m => ModelResultToResponse.Create(m.Settings.UnitSettings).Map(m.AnalyticalResults)
            )
            .FirstOrDefaultAsync(ct);
    }
}
