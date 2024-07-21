using BeamOs.Application.PhysicalModel.Models;
using BeamOs.Domain.Common.ValueObjects;
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

        return await queryable.FirstOrDefaultAsync(el => el.Id == id, ct);
    }

    public override Task<Model?> GetById(ModelId id, CancellationToken ct = default)
    {
        return this.DbContext
            .Models
            .Include(m => m.Nodes)
            .ThenInclude(n => n.PointLoads)
            .Include(m => m.Nodes)
            .ThenInclude(n => n.MomentLoads)
            .Include(m => m.Element1ds)
            .ThenInclude(m => m.StartNode)
            .Include(m => m.Element1ds)
            .ThenInclude(m => m.EndNode)
            .Include(m => m.Materials)
            .Include(m => m.SectionProfiles)
            .FirstOrDefaultAsync(el => el.Id == id, ct);
    }

    public async Task<UnitSettings> GetUnits(ModelId id, CancellationToken ct) =>
        (await this.DbContext.Models.FirstOrDefaultAsync(el => el.Id == id, ct))
            ?.Settings
            .UnitSettings ?? throw new Exception($"Could not find model with id {id.Id}");
}
