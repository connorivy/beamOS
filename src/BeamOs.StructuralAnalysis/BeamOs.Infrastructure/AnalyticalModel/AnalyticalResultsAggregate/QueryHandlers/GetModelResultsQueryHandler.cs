using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.AnalyticalModel.Results;
using BeamOs.Contracts.Common;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Infrastructure.QueryHandlers.AnalyticalModel.ModelResults.Mappers;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.AnalyticalModel.AnalyticalResultsAggregate.QueryHandlers;

public sealed class GetModelResultsQueryHandler(BeamOsStructuralDbContext dbContext)
    : IQueryHandler<ModelIdRequest, AnalyticalResultsResponse>
{
    public async Task<AnalyticalResultsResponse?> ExecuteAsync(
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
