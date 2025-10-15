using BeamOs.Identity;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Models;

internal sealed class ModelRepository(
    StructuralAnalysisDbContext dbContext,
    IUserIdProvider userIdProvider
) : RepositoryBase<ModelId, Model>(dbContext), IModelRepository
{
    public override void Add(Model aggregate)
    {
        aggregate.AddEvent(new ModelCreatedEvent(aggregate.Id, userIdProvider.UserId));
        base.Add(aggregate);
    }

    public void AddTempModel(Model aggregate)
    {
        this.DbContext.Models.Add(aggregate);
    }

    public async Task<Model?> GetSingle(
        ModelId modelId,
        Func<IQueryable<Model>, IQueryable<Model>>? includeNavigationProperties = null,
        CancellationToken ct = default
    )
    {
        IQueryable<Model> queryable = this.DbContext.Models;

        if (includeNavigationProperties is not null)
        {
            queryable = includeNavigationProperties(queryable);
        }

        return await queryable
            .AsNoTracking()
            .AsSplitQuery()
            .FirstOrDefaultAsync(el => el.Id == modelId, ct);
    }

    public async Task<Model?> GetSingle(
        ModelId modelId,
        CancellationToken ct = default,
        params string[] includeNavigationProperties
    )
    {
        IQueryable<Model> queryable = this.DbContext.Models;

        foreach (var prop in includeNavigationProperties)
        {
            queryable = queryable.Include(prop);
        }

        return await queryable
            .AsNoTracking()
            .AsSplitQuery()
            .FirstOrDefaultAsync(el => el.Id == modelId, ct);
    }
}
