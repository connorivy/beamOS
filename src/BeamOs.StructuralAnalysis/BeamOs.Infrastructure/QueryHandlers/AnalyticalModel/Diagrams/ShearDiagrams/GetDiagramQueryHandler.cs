using BeamOs.Application.Common.Queries;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Domain.Diagrams.MomentDiagramAggregate;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;
using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.QueryHandlers.AnalyticalModel.Diagrams.ShearDiagrams;

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

internal class GetMomentDiagramQueryHandler(BeamOsStructuralDbContext dbContext)
    : IQueryHandler<GetResourceByIdQuery, MomentDiagram>
{
    private readonly BeamOsStructuralDbContext dbContext = dbContext;

    public async Task<MomentDiagram?> ExecuteAsync(
        GetResourceByIdQuery query,
        CancellationToken ct = default
    )
    {
        return await this.dbContext
            .MomentDiagrams
            .AsNoTracking()
            .Include(el => el.Intervals.OrderBy(i => i.StartLocation))
            .Where(s => s.Element1DId == new Element1DId(query.Id))
            .FirstOrDefaultAsync(cancellationToken: ct);
    }
}

internal class GetMomentDiagramsQueryHandler(BeamOsStructuralDbContext dbContext)
    : IQueryHandler<GetResourceByIdQuery, MomentDiagram[]>
{
    private readonly BeamOsStructuralDbContext dbContext = dbContext;

    public async Task<MomentDiagram[]> ExecuteAsync(
        GetResourceByIdQuery query,
        CancellationToken ct = default
    )
    {
        return await this.dbContext
            .Models
            .AsNoTracking()
            .Include(m => m.AnalyticalResults)
            .ThenInclude(r => r.MomentDiagrams)
            .ThenInclude(d => d.Intervals.OrderBy(i => i.StartLocation))
            .Where(m => m.Id == new ModelId(query.Id))
            .SelectMany(m => m.AnalyticalResults.MomentDiagrams)
            .ToArrayAsync(cancellationToken: ct);
    }
}

internal class GetShearDiagramsQueryHandler(BeamOsStructuralDbContext dbContext)
    : IQueryHandler<GetResourceByIdQuery, ShearForceDiagram[]>
{
    private readonly BeamOsStructuralDbContext dbContext = dbContext;

    public async Task<ShearForceDiagram[]> ExecuteAsync(
        GetResourceByIdQuery query,
        CancellationToken ct = default
    )
    {
        return await this.dbContext
            .ShearForceDiagrams
            .AsNoTracking()
            .Include(el => el.Intervals.OrderBy(i => i.StartLocation))
            .Include(el => el.Element1d)
            .Where(m => m.Element1d.ModelId == new ModelId(query.Id))
            .ToArrayAsync(cancellationToken: ct);
    }
}
