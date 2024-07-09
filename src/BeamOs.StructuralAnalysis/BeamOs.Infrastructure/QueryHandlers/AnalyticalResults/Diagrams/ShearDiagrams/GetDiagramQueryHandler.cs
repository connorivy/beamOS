using BeamOs.Application.AnalyticalResults.Diagrams.ShearDiagrams.Interfaces;
using BeamOs.Application.Common.Queries;
using BeamOs.Common.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.QueryHandlers.AnalyticalResults.Diagrams.ShearDiagrams;

internal class GetDiagramQueryHandler(BeamOsStructuralReadModelDbContext dbContext)
    : IQueryHandler<GetResourceByIdQuery, IShearDiagramData>
{
    private readonly BeamOsStructuralReadModelDbContext dbContext = dbContext;

    public async Task<IShearDiagramData?> ExecuteAsync(
        GetResourceByIdQuery query,
        CancellationToken ct = default
    )
    {
        return null;
        //return await this.dbContext
        //    .ShearForceDiagrams
        //    .Where(s => s.Element1DId == query.Id)
        //    .FirstOrDefaultAsync(cancellationToken: ct);
    }
}
