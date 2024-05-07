using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.Common.Queries;
using BeamOs.Application.PhysicalModel.Element1dAggregate.Interfaces;
using BeamOs.Contracts.AnalyticalResults.AnalyticalNode;
using BeamOs.Domain.Common.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.QueryHandlers.AnalyticalResults.NodeResults;

internal class GetNodeResultsToResponseQueryHandler
    : IQueryHandler<GetResourcesByIdsQuery, NodeResultResponse?[]>
{
    private readonly BeamOsStructuralReadModelDbContext dbContext;

    public GetNodeResultsToResponseQueryHandler(BeamOsStructuralReadModelDbContext dbContext) =>
        this.dbContext = dbContext;

    public async Task<NodeResultResponse?[]?> ExecuteAsync(
        GetResourcesByIdsQuery query,
        CancellationToken ct = default
    )
    {
        UnitSettings unitSettings = this.dbContext.Models.Where(m => m.Id == query.Id)
        var element1dQuery = this.dbContext.NodeResults.Where(el => query.Ids.Contains(el.NodeId));

        return await element1dQuery.FirstOrDefaultAsync(ct);
    }
}
