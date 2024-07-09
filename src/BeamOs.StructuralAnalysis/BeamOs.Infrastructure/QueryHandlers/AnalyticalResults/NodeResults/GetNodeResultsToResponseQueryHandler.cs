using BeamOs.Application.Common.Queries;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.AnalyticalResults.AnalyticalNode;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Infrastructure.Data.Models;
using BeamOs.Infrastructure.QueryHandlers.AnalyticalResults.NodeResults.Mappers;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.QueryHandlers.AnalyticalResults.NodeResults;

internal class GetNodeResultsToResponseQueryHandler(BeamOsStructuralReadModelDbContext dbContext)
    : IQueryHandler<GetModelResourcesByIdsQuery, NodeResultResponse?[]>
{
    public async Task<NodeResultResponse?[]?> ExecuteAsync(
        GetModelResourcesByIdsQuery query,
        CancellationToken ct = default
    )
    {
        IQueryable<NodeResultReadModel> queryable = dbContext
            .NodeResults
            .Where(el => el.ModelId == query.ModelId);

        if (query.ResourceIds is not null)
        {
            queryable = queryable.Where(el => query.ResourceIds.Contains(el.Id));
        }

        NodeResultReadModel[] nodeResultReadModels = await queryable.ToArrayAsync(
            cancellationToken: ct
        );

        if (nodeResultReadModels.Length == 0)
        {
            return [];
        }

        UnitSettings unitSettings = await dbContext
            .Models
            .Where(m => m.Id == query.ModelId)
            .Select(m => m.Settings.UnitSettings)
            .FirstAsync(cancellationToken: ct);

        var responseMapper = NodeResultReadModelToResponse.Create(unitSettings);

        return responseMapper.Map(nodeResultReadModels).ToArray();
    }
}
