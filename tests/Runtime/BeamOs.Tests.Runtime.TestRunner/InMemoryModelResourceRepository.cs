using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.Tests.Runtime.TestRunner;

public class InMemoryRepository<TId, T> : IRepository<TId, T>
    where TId : struct
    where T : BeamOsEntity<TId>
{
    protected Dictionary<TId, T> Entities { get; } = [];

    public event EventHandler<TId>? EntityCreated;

    public void Add(T aggregate)
    {
        this.Entities.Add(aggregate.Id, aggregate);
        this.EntityCreated?.Invoke(this, aggregate.Id);
    }

    public void Put(T aggregate)
    {
        this.Entities[aggregate.Id] = aggregate;
    }

    public void Remove(T aggregate)
    {
        this.Entities.Remove(aggregate.Id);
    }

    public void ClearChangeTracker()
    {
        // No-op for in-memory repository
    }
}

public class InMemoryModelResourceRepository<TId, T>
    : InMemoryRepository<TId, T>,
        IModelResourceRepository<TId, T>
    where TId : struct, IIntBasedId
    where T : BeamOsModelEntity<TId>
{
    public Task<List<TId>> GetIdsInModel(ModelId modelId, CancellationToken ct = default)
    {
        var ids = this.Entities.Where(x => x.Value.ModelId == modelId).Select(x => x.Key).ToList();

        return Task.FromResult(ids);
    }

    public Task<List<T>> GetMany(ModelId modelId, IList<TId>? ids, CancellationToken ct = default)
    {
        return Task.FromResult(ids is null ? [] : ids.Select(x => this.Entities[x]).ToList());
    }

    public Task<T?> GetSingle(ModelId modelId, TId id, CancellationToken ct = default)
    {
        return Task.FromResult(this.Entities.TryGetValue(id, out var entity) ? entity : null);
    }

    public Task RemoveById(ModelId modelId, TId id, CancellationToken ct = default)
    {
        this.Entities.Remove(id);
        return Task.CompletedTask;
    }

    public Task ReloadEntity(T entity, CancellationToken ct = default)
    {
        // No-op for in-memory repository
        return Task.CompletedTask;
    }
}

public class InMemoryAnalyticalResultRepository<TId, T>
    : InMemoryRepository<TId, T>,
        IAnalyticalResultRepository<TId, T>
    where TId : struct
    where T : BeamOsEntity<TId>
{
    public Task<T?> GetSingle(ModelId modelId, ResultSetId resultSetId, TId id)
    {
        return Task.FromResult(this.Entities.TryGetValue(id, out var entity) ? entity : null);
    }
}

public class InMemoryProposalRepository<TId, T>
    : InMemoryRepository<TId, T>,
        IProposalRepository<TId, T>
    where TId : struct
    where T : BeamOsEntity<TId>
{
    public Task<T?> GetSingle(ModelId modelId, ModelProposalId modelProposalId, TId id)
    {
        return Task.FromResult(this.Entities.TryGetValue(id, out var entity) ? entity : null);
    }
}
