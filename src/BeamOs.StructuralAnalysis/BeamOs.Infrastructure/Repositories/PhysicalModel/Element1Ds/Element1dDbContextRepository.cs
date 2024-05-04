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
}
