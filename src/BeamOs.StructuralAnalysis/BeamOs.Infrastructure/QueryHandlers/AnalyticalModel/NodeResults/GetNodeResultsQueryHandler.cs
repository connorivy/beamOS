using BeamOs.Application.Common.Queries;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.AnalyticalModel.AnalyticalNode;
using BeamOs.Contracts.Common;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOs.Infrastructure.QueryHandlers.AnalyticalResultReadModels.NodeResultReadModels.Mappers;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.QueryHandlers.AnalyticalModel.NodeResults;

internal class GetNodeResultsQueryHandler(
    BeamOsStructuralDbContext dbContext,
    IQueryHandler<GetResourceByIdQuery, UnitSettings> unitSettingsQuery
) : IQueryHandler<IdRequest, NodeResultResponse[]>
{
    public async Task<NodeResultResponse[]?> ExecuteAsync(
        IdRequest query,
        CancellationToken ct = default
    )
    {
        var nodeResults = await dbContext
            .NodeResults
            .AsNoTracking()
            .Where(el => el.NodeId == new NodeId(Guid.Parse(query.Id)))
            .ToArrayAsync(ct);

        if (nodeResults.Length == 0)
        {
            return [];
        }

        UnitSettings unitSettings = await dbContext
            .Models
            .Include(m => m.AnalyticalResults)
            .Where(m => m.AnalyticalResults.Id == nodeResults[0].ModelResultId)
            .Select(m => m.Settings.UnitSettings)
            .FirstAsync(cancellationToken: ct);

        var responseMapper = NodeResultToResponse.Create(unitSettings);

        return responseMapper.Map(nodeResults).ToArray();
    }
}
