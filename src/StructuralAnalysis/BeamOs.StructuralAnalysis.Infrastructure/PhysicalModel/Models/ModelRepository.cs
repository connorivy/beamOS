using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Models;

internal class ModelRepository(StructuralAnalysisDbContext dbContext) : IModelRepository
{
    public void Add(Model entity) => dbContext.Models.Add(entity);

    public async Task<Model?> GetSingle(
        ModelId modelId,
        Func<IQueryable<Model>, IQueryable<Model>>? includeNavigationProperties = null,
        CancellationToken ct = default
    )
    {
        IQueryable<Model> queryable = dbContext.Models;

        if (includeNavigationProperties is not null)
        {
            queryable = includeNavigationProperties(queryable);
        }

        return await queryable.AsSplitQuery().FirstOrDefaultAsync(el => el.Id == modelId, ct);
    }

    public async Task<Model?> GetSingle(
        ModelId modelId,
        CancellationToken ct = default,
        params string[] includeNavigationProperties
    )
    {
        IQueryable<Model> queryable = dbContext.Models;

        foreach (var prop in includeNavigationProperties)
        {
            queryable = queryable.Include(prop);
        }

        return await queryable.AsSplitQuery().FirstOrDefaultAsync(el => el.Id == modelId, ct);
    }
}
