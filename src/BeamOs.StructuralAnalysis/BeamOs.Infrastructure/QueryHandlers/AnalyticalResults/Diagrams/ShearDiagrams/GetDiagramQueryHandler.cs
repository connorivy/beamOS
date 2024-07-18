using BeamOs.Application.Common.Queries;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;
using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.QueryHandlers.AnalyticalResults.Diagrams.ShearDiagrams;

internal class GetDiagramQueryHandler(BeamOsStructuralDbContext dbContext)
    : IQueryHandler<GetResourceByIdQuery, ShearForceDiagram>
{
    private readonly BeamOsStructuralDbContext dbContext = dbContext;

    public async Task<ShearForceDiagram?> ExecuteAsync(
        GetResourceByIdQuery query,
        CancellationToken ct = default
    )
    {
        return await this.dbContext
            .ShearForceDiagrams
            .AsNoTracking()
            .Include(el => el.Intervals.OrderBy(i => i.StartLocation))
            .Where(s => s.Element1DId == new Element1DId(query.Id))
            .FirstOrDefaultAsync(cancellationToken: ct);
    }
}
