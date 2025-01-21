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

    public void Add(TEntity aggregate)
    {
        _ = dbContext.Set<TEntity>().Add(aggregate);
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
}

internal abstract class ModelResourceRepositoryBase<TId, TEntity>(
    StructuralAnalysisDbContext dbContext
) : RepositoryBase<TId, TEntity>(dbContext), IModelResourceRepository<TId, TEntity>
    where TId : struct, IEquatable<TId>, IIntBasedId
    where TEntity : BeamOsModelEntity<TId>
{
    public async Task<TEntity?> GetSingle(ModelId modelId, TId id) =>
        await this.DbContext
            .Set<TEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.ModelId == modelId && m.Id.Equals((TId)id));
}

internal abstract class AnalyticalResultRepositoryBase<TId, TEntity>(
    StructuralAnalysisDbContext dbContext
) : RepositoryBase<TId, TEntity>(dbContext), IAnalyticalResultRepository<TId, TEntity>
    where TId : struct, IEquatable<TId>
    where TEntity : BeamOsAnalyticalResultEntity<TId>
{
    public async Task<TEntity?> GetSingle(ModelId modelId, ResultSetId resultSetId, TId id) =>
        await this.DbContext
            .Set<TEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                m => m.ModelId == modelId && m.ResultSetId == resultSetId && m.Id.Equals(id)
            );
}
