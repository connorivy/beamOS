using System.Threading;
using BeamOs.Application.PhysicalModel.Element1dAggregate;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.Repositories.PhysicalModel.Element1Ds;

internal class Element1dDbContextRepository(BeamOsStructuralDbContext dbContext)
    : RepositoryBase<Element1DId, Element1D>(dbContext),
        IElement1dRepository
{
    public async Task<List<Element1D>> GetElement1dsInModel(
        ModelId modelId,
        CancellationToken ct = default
    )
    {
        return await this.DbContext
            .Element1Ds
            .Where(el => el.ModelId == modelId)
            .ToListAsync(cancellationToken: ct);
    }

    public override Task<Element1D?> GetById(Element1DId id, CancellationToken ct = default) =>
        this.DbContext
            .Element1Ds
            .Include(el => el.StartNode)
            .ThenInclude(el => el.PointLoads)
            .Include(el => el.StartNode)
            .ThenInclude(el => el.MomentLoads)
            .Include(el => el.StartNode)
            .ThenInclude(el => el.NodeResult)
            .Include(el => el.EndNode)
            .ThenInclude(el => el.PointLoads)
            .Include(el => el.EndNode)
            .ThenInclude(el => el.MomentLoads)
            .Include(el => el.EndNode)
            .ThenInclude(el => el.NodeResult)
            .Include(el => el.Material)
            .Include(el => el.SectionProfile)
            .AsSplitQuery()
            .FirstOrDefaultAsync(el => el.Id == id, cancellationToken: ct);
}
