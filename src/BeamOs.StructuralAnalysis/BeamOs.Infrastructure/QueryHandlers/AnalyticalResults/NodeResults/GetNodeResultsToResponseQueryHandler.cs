using BeamOs.Application.Common.Queries;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.AnalyticalResults.AnalyticalNode;
using BeamOs.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Infrastructure.QueryHandlers.AnalyticalResultReadModels.NodeResultReadModels.Mappers;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.QueryHandlers.AnalyticalResults.NodeResults;

internal class GetNodeResultsToResponseQueryHandler(BeamOsStructuralDbContext dbContext)
    : IQueryHandler<GetModelResourcesByIdsQuery, NodeResultResponse[]>
{
    public async Task<NodeResultResponse[]?> ExecuteAsync(
        GetModelResourcesByIdsQuery query,
        CancellationToken ct = default
    )
    {
        ModelId modelId = new(query.ModelId);
        IQueryable<NodeResult> queryable = dbContext.NodeResults.Where(el => el.ModelId == modelId);

        if (query.ResourceIds is not null && query.ResourceIds.Count > 0)
        {
            queryable = queryable.Where(el => query.ResourceIds.Contains(el.Id.Id));
        }

        NodeResult[] nodeResultReadModels = await queryable.ToArrayAsync(cancellationToken: ct);

        if (nodeResultReadModels.Length == 0)
        {
            return [];
        }

        UnitSettings unitSettings = await dbContext
            .Models
            .Where(m => m.Id == modelId)
            .Select(m => m.Settings.UnitSettings)
            .FirstAsync(cancellationToken: ct);

        var responseMapper = NodeResultToResponse.Create(unitSettings);

        return responseMapper.Map(nodeResultReadModels).ToArray();
    }
}
