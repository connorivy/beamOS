using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.AnalyticalResults.Model;
using BeamOs.Contracts.Common;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Infrastructure;
using BeamOs.Infrastructure.QueryHandlers.AnalyticalResults.ModelResults.Mappers;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Application.AnalyticalResults.ModelResults.Queries;

public sealed class GetModelResultsQueryHandler(BeamOsStructuralDbContext dbContext)
    : IQueryHandler<IdRequest, ModelResultResponse>
{
    public async Task<ModelResultResponse?> ExecuteAsync(
        IdRequest query,
        CancellationToken ct = default
    )
    {
        ModelId modelId = new(Guid.Parse(query.Id));
        return await dbContext
            .Models
            .Include(m => m.ModelResults.Take(1))
            .Where(m => m.Id == modelId)
            .Select(
                m =>
                    ModelResultToResponse
                        .Create(m.Settings.UnitSettings)
                        .Map(m.ModelResults.First())
            )
            .FirstOrDefaultAsync(ct);
    }
}
