using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.StructuralAnalysis.Infrastructure.Common;

internal abstract class RepositoryBase<TId, TEntity>(StructuralAnalysisDbContext dbContext)
    : IRepository<TId, TEntity>
    where TId : struct
    where TEntity : BeamOsEntity<TId>
{
    protected StructuralAnalysisDbContext DbContext => dbContext;

    public virtual void Add(TEntity aggregate)
    {
        _ = dbContext.Set<TEntity>().Add(aggregate);
    }

    public virtual ValueTask Put(TEntity aggregate)
    {
        _ = dbContext.Set<TEntity>().Update(aggregate);
        return ValueTask.CompletedTask;
    }

    public void Remove(TEntity aggregate)
    {
        //aggregate.AddEvent(new ModelDeletedEvent(aggregate.Id));
        _ = dbContext.Set<TEntity>().Remove(aggregate);
    }

    public void Update(TEntity aggregate)
    {
        _ = dbContext.Set<TEntity>().Update(aggregate);
    }

    public void ClearChangeTracker()
    {
        dbContext.ChangeTracker.Clear();
    }
}

internal abstract class ModelResourceRepositoryInBase<TId, TEntity>(
    StructuralAnalysisDbContext dbContext
) : RepositoryBase<TId, TEntity>(dbContext), IModelResourceRepositoryIn<TId, TEntity>
    where TId : struct, IEquatable<TId>, IIntBasedId
    where TEntity : BeamOsModelEntity<TId>
{
    public async Task<List<TId>> GetIdsInModel(ModelId modelId, CancellationToken ct = default) =>
        await this
            .DbContext.Set<TEntity>()
            .Where(m => m.ModelId == modelId)
            .Select(m => m.Id)
            .ToListAsync(ct);

    public async Task RemoveById(ModelId modelId, TId id, CancellationToken ct = default)
    {
        var entity = await this.DbContext.Set<TEntity>().FindAsync([id, modelId], ct);
        if (entity is not null)
        {
            this.DbContext.Set<TEntity>().Remove(entity);
        }
    }

    public async Task ReloadEntity(TEntity entity, CancellationToken ct = default)
    {
        await this.DbContext.Entry(entity).ReloadAsync(ct);
    }
}

internal abstract class ModelResourceRepositoryBase<TId, TEntity>(
    StructuralAnalysisDbContext dbContext
) : ModelResourceRepositoryInBase<TId, TEntity>(dbContext), IModelResourceRepository<TId, TEntity>
    where TId : struct, IEquatable<TId>, IIntBasedId
    where TEntity : BeamOsModelEntity<TId>
{
    public virtual async Task<TEntity?> GetSingle(
        ModelId modelId,
        TId id,
        CancellationToken ct = default
    )
    {
        var entity = this.DbContext.Set<TEntity>();
        return await entity
            .AsNoTracking()
            .FirstOrDefaultAsync(
                m => m.ModelId == modelId && m.Id.Equals(id),
                cancellationToken: ct
            );
    }

    public virtual async Task<ModelSettingsAndEntity<TEntity>?> GetSingleWithModelSettings(
        ModelId modelId,
        TId id,
        CancellationToken ct = default
    )
    {
        var settingAndEntity = await this
            .DbContext.Set<TEntity>()
            .AsNoTracking()
            .Where(m => m.ModelId == modelId && m.Id.Equals(id))
            .Select(m => new { ModelSettings = m.Model.Settings, Entity = m })
            .FirstOrDefaultAsync(ct);

        return settingAndEntity is null
            ? null
            : new ModelSettingsAndEntity<TEntity>(
                settingAndEntity.ModelSettings,
                settingAndEntity.Entity
            );
    }

    public virtual Task<List<TEntity>> GetMany(
        ModelId modelId,
        IList<TId>? ids,
        CancellationToken ct = default
    )
    {
        if (ids is null)
        {
            return this.DbContext.Set<TEntity>().Where(m => m.ModelId == modelId).ToListAsync(ct);
        }
        return this
            .DbContext.Set<TEntity>()
            .Where(m => m.ModelId == modelId && ids.Contains(m.Id))
            .ToListAsync(ct);
    }
}

internal abstract class AnalyticalResultRepositoryBase<TId, TEntity>(
    StructuralAnalysisDbContext dbContext
) : RepositoryBase<TId, TEntity>(dbContext), IAnalyticalResultRepository<TId, TEntity>
    where TId : struct, IEquatable<TId>
    where TEntity : BeamOsAnalyticalResultEntity<TId>
{
    public async Task<TEntity?> GetSingle(ModelId modelId, ResultSetId resultSetId, TId id) =>
        await this
            .DbContext.Set<TEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(m =>
                m.ModelId == modelId && m.ResultSetId == resultSetId && m.Id.Equals(id)
            );

    public async Task<ModelSettingsAndEntity<TEntity>?> GetSingleWithModelSettings(
        ModelId modelId,
        TId id,
        CancellationToken ct = default
    )
    {
        var settingAndEntity = await this
            .DbContext.Set<TEntity>()
            .AsNoTracking()
            .Where(m => m.ModelId == modelId && m.Id.Equals(id))
            .Select(m => new { ModelSettings = m.Model.Settings, Entity = m })
            .FirstOrDefaultAsync(ct);

        return settingAndEntity is null
            ? null
            : new ModelSettingsAndEntity<TEntity>(
                settingAndEntity.ModelSettings,
                settingAndEntity.Entity
            );
    }

    public async Task<ModelSettingsAndEntity<TEntity[]>?> GetAllFromLoadCombinationWithModelSettings(
        ModelId modelId,
        ResultSetId resultSetId,
        CancellationToken ct = default
    )
    {
        var modelSettings = await this
            .DbContext.Set<Model>()
            .AsNoTracking()
            .Where(m => m.Id == modelId)
            .Select(m => m.Settings)
            .FirstAsync(ct);

        var settingAndEntity = await this
            .DbContext.Set<TEntity>()
            .AsNoTracking()
            .Where(m => m.ModelId == modelId && m.ResultSetId == resultSetId)
            .ToArrayAsync(ct);

        return settingAndEntity is null
            ? null
            : new ModelSettingsAndEntity<TEntity[]>(modelSettings, settingAndEntity);
    }
}
