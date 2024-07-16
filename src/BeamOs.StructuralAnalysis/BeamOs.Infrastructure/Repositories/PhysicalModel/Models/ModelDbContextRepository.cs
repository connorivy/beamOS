using BeamOs.Application.PhysicalModel.Models;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.Repositories.PhysicalModel.Models;

internal class ModelDbContextRepository(BeamOsStructuralDbContext dbContext)
    : RepositoryBase<ModelId, Model>(dbContext),
        IModelRepository
{
    public async Task<Model?> GetById(
        ModelId id,
        CancellationToken ct = default,
        params string[] propertiesToLoad
    )
    {
        IQueryable<Model> queryable = this.DbContext.Models;

        foreach (var property in propertiesToLoad)
        {
            queryable = queryable.Include(property);
        }

        return await queryable.FirstOrDefaultAsync(ct);
    }

    public override async Task RemoveById(ModelId id, CancellationToken ct = default)
    {
        await dbContext
            .Models
            .Include(m => m.Element1ds)
            .Include(m => m.Materials)
            .Where(e => e.Id == id)
            .ExecuteDeleteAsync(cancellationToken: ct);
    }
}
