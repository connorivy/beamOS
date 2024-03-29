using BeamOS.Common.Application.Interfaces;
using BeamOS.Common.Domain.Models;

namespace BeamOS.Common.Application;

public class InMemoryRepository<TId, T> : IRepository<TId, T>
    where TId : notnull
    where T : AggregateRoot<TId>
{
    private readonly Dictionary<TId, T> values = [];

    public Task Add(T aggregate)
    {
        _ = this.values.TryAdd(aggregate.Id, aggregate);
        return Task.CompletedTask;
    }

    public async Task<T?> GetById(TId id)
    {
        await Task.CompletedTask;

        _ = this.values.TryGetValue(id, out var result);
        return result;
    }
}
