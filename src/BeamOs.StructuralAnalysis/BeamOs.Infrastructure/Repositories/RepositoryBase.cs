using BeamOs.Common.Application.Interfaces;
using BeamOs.Common.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.Repositories;

internal abstract class RepositoryBase<TId, TEntity>(BeamOsStructuralDbContext dbContext)
    : IRepository<TId, TEntity>
    where TId : class
    where TEntity : AggregateRoot<TId>
{
    protected BeamOsStructuralDbContext DbContext => dbContext;

    public void Add(TEntity aggregate)
    {
        _ = dbContext.Set<TEntity>().Add(aggregate);
    }

    public void Remove(TEntity aggregate)
    {
        _ = dbContext.Set<TEntity>().Remove(aggregate);
    }

    public virtual async Task<TEntity?> GetById(TId id, CancellationToken ct = default)
    {
        return await dbContext
            .Set<TEntity>()
            .FirstOrDefaultAsync(el => el.Id == id, cancellationToken: ct);
    }

    public void Update(TEntity aggregate)
    {
        _ = dbContext.Set<TEntity>().Update(aggregate);
    }

    public virtual async Task RemoveById(TId id, CancellationToken ct = default)
    {
        await dbContext
            .Set<TEntity>()
            .Where(e => e.Id == id)
            .ExecuteDeleteAsync(cancellationToken: ct);
    }
}
