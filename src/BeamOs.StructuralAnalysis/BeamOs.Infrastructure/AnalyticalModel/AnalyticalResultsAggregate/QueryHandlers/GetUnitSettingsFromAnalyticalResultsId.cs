using BeamOs.Common.Application.Interfaces;
using BeamOs.Domain.AnalyticalModel.AnalyticalResultsAggregate.ValueObjects;
using BeamOs.Domain.Common.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.QueryHandlers.AnalyticalModel.ModelResults;

public sealed class GetModelResultsQueryHandler(BeamOsStructuralDbContext dbContext)
    : IQueryHandler<AnalyticalResultsId, UnitSettings>
{
    public async Task<UnitSettings?> ExecuteAsync(
        AnalyticalResultsId query,
        CancellationToken ct = default
    )
    {
        return await dbContext
            .Models
            .Include(m => m.AnalyticalResults)
            .Where(m => m.AnalyticalResults.Id == query)
            .Select(m => m.Settings.UnitSettings)
            .FirstOrDefaultAsync(ct);
    }
}
