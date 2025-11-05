using System.Collections.Concurrent;
using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Application.Common;

internal class InMemoryRepository<TId, T> : IRepository<TId, T>
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

    public ValueTask Put(T aggregate)
    {
        this.Entities[aggregate.Id] = aggregate;
        return ValueTask.CompletedTask;
    }

    public void Remove(T aggregate)
    {
        this.Entities.Remove(aggregate.Id);
    }

    public void ClearChangeTracker()
    {
        // No-op for in-memory repository
    }

    public void Attach(T aggregate)
    {
        // No-op for in-memory repository
    }
}

internal class InMemoryModelResourceRepository<TId, T>(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
) : IModelResourceRepository<TId, T>
    where TId : struct, IIntBasedId
    where T : BeamOsModelEntity<TId>
{
    protected Dictionary<ModelId, Dictionary<TId, T>> ModelResources { get; } =
        inMemoryModelRepositoryStorage.GetStorage<Dictionary<ModelId, Dictionary<TId, T>>>();

    public virtual void Add(T aggregate)
    {
        if (!this.ModelResources.TryGetValue(aggregate.ModelId, out var resources))
        {
            resources = [];
            this.ModelResources[aggregate.ModelId] = resources;
        }
        TId id = aggregate.Id.Id == 0 ? new() { Id = resources.Count + 1 } : aggregate.Id;
        aggregate.__SetId(id);
        resources[id] = aggregate;
    }

    public void ClearChangeTracker()
    {
        // No-op for in-memory repository
    }

    public void Attach(T aggregate)
    {
        // No-op for in-memory repository
    }

    public Task<List<TId>> GetIdsInModel(ModelId modelId, CancellationToken ct = default)
    {
        if (this.ModelResources.TryGetValue(modelId, out var resources))
        {
            return Task.FromResult(new List<TId>(resources.Keys));
        }
        return Task.FromResult(new List<TId>());
    }

    public virtual Task<List<T>> GetMany(
        ModelId modelId,
        IList<TId>? ids,
        CancellationToken ct = default
    )
    {
        if (!this.ModelResources.TryGetValue(modelId, out var resources))
        {
            return Task.FromResult(new List<T>());
        }

        if (ids is null)
        {
            return Task.FromResult(new List<T>(resources.Values));
        }

        var result = new List<T>();
        foreach (var id in ids)
        {
            if (resources.TryGetValue(id, out var entity))
            {
                result.Add(entity);
            }
        }
        return Task.FromResult(result);
    }

    public virtual Task<T?> GetSingle(ModelId modelId, TId id, CancellationToken ct = default)
    {
        if (
            this.ModelResources.TryGetValue(modelId, out var resources)
            && resources.TryGetValue(id, out var entity)
        )
        {
            return Task.FromResult<T?>(entity);
        }
        return Task.FromResult<T?>(null);
    }

    public Task<ModelSettingsAndEntity<T>?> GetSingleWithModelSettings(
        ModelId modelId,
        TId id,
        CancellationToken ct = default
    )
    {
        var model = inMemoryModelRepositoryStorage.Models.GetValueOrDefault(modelId);
        if (model is null)
        {
            return Task.FromResult(default(ModelSettingsAndEntity<T>?)); // Model not found
        }

        if (
            this.ModelResources.TryGetValue(modelId, out var resources)
            && resources.TryGetValue(id, out var entity)
        )
        {
            return Task.FromResult<ModelSettingsAndEntity<T>?>(
                new ModelSettingsAndEntity<T>(model.Settings, entity)
            );
        }

        return Task.FromResult(default(ModelSettingsAndEntity<T>?)); // Entity not found
    }

    public virtual ValueTask Put(T aggregate)
    {
        if (this.ModelResources.TryGetValue(aggregate.ModelId, out var resources))
        {
            resources[aggregate.Id] = aggregate;
        }
        else
        {
            throw new KeyNotFoundException($"Model {aggregate.ModelId} not found.");
        }
        return ValueTask.CompletedTask;
    }

    public Task ReloadEntity(T entity, CancellationToken ct = default)
    {
        // This method is not implemented for in-memory repository
        return Task.CompletedTask;
    }

    public virtual void Remove(T aggregate)
    {
        this.RemoveById(aggregate.ModelId, aggregate.Id);
    }

    public virtual Task RemoveById(ModelId modelId, TId id, CancellationToken ct = default)
    {
        if (this.ModelResources.TryGetValue(modelId, out var resources))
        {
            resources.Remove(id);
            if (resources.Count == 0)
            {
                this.ModelResources.Remove(modelId);
            }
        }
        return Task.CompletedTask;
    }
}

internal class InMemoryAnalyticalResultRepository<TId, T>(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
)
    : InMemoryModelResourceRepository<TId, T>(inMemoryModelRepositoryStorage),
        IAnalyticalResultRepository<TId, T>
    where TId : struct, IIntBasedId
    where T : BeamOsAnalyticalResultEntity<TId>
{
    public Task<T?> GetSingle(ModelId modelId, ResultSetId resultSetId, TId id)
    {
        var model = inMemoryModelRepositoryStorage.Models.GetValueOrDefault(modelId);
        if (model is null)
        {
            return Task.FromResult<T?>(null); // Model not found
        }

        if (
            this.ModelResources.TryGetValue(modelId, out var resources)
            && resources.TryGetValue(id, out var entity)
        )
        {
            return Task.FromResult(entity);
        }
        return Task.FromResult<T?>(null); // Entity not found
    }

    public Task<ModelSettingsAndEntity<T>?> GetSingleWithModelSettings(
        ModelId modelId,
        TId id,
        CancellationToken ct = default
    )
    {
        var model = inMemoryModelRepositoryStorage.Models.GetValueOrDefault(modelId);
        if (model is null)
        {
            return Task.FromResult<ModelSettingsAndEntity<T>?>(null); // Model not found
        }

        if (
            this.ModelResources.TryGetValue(modelId, out var resources)
            && resources.TryGetValue(id, out var entity)
        )
        {
            return Task.FromResult<ModelSettingsAndEntity<T>?>(new(model.Settings, entity));
        }
        return Task.FromResult<ModelSettingsAndEntity<T>?>(null); // Entity not found
    }

    public Task<ModelSettingsAndEntity<T[]>?> GetAllFromLoadCombinationWithModelSettings(
        ModelId modelId,
        ResultSetId resultSetId,
        CancellationToken ct = default
    )
    {
        var model = inMemoryModelRepositoryStorage.Models.GetValueOrDefault(modelId);
        if (model is null)
        {
            return Task.FromResult<ModelSettingsAndEntity<T[]>?>(null); // Model not found
        }

        if (this.ModelResources.TryGetValue(modelId, out var resources))
        {
            return Task.FromResult<ModelSettingsAndEntity<T[]>?>(
                new(
                    model.Settings,
                    resources.Values.Where(r => r.ResultSetId == resultSetId).ToArray()
                )
            );
        }
        return Task.FromResult<ModelSettingsAndEntity<T[]>?>(null); // Entity not found
    }
}

internal class InMemoryProposalRepository<TId, T>
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

internal record InMemoryModelRepositoryStorage
{
    public Dictionary<ModelId, Model> Models { get; } = [];
    private readonly ConcurrentDictionary<Type, object> resourceStorage = [];

    public T GetStorage<T>()
        where T : class, new()
    {
        var type = typeof(T);
        if (!this.resourceStorage.TryGetValue(type, out var storage))
        {
            storage = new T();
            this.resourceStorage[type] = storage;
        }
        return (T)storage;
    }
}
