using BeamOs.Common.Application.Interfaces;
using BeamOs.Common.Domain.Models;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.Events;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.Repositories;

internal abstract class RepositoryBase<TId, TEntity>(BeamOsStructuralDbContext dbContext)
    : IRepository<TId, TEntity>
    where TId : GuidBasedId
    where TEntity : AggregateRoot<TId>
{
    protected BeamOsStructuralDbContext DbContext => dbContext;

    public void Add(TEntity aggregate)
    {
        _ = dbContext.Set<TEntity>().Add(aggregate);
    }

    public void Remove(TEntity aggregate)
    {
        aggregate.AddEvent(new ModelDeletedEvent(aggregate.Id));
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
        var entityToDelete = await dbContext
            .Set<TEntity>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken: ct);

        if (entityToDelete is null)
        {
            return;
        }

        this.Remove(entityToDelete);
    }
}
