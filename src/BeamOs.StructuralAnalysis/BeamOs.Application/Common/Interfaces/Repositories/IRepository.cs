using BeamOs.Domain.Common.Models;

namespace BeamOs.Application.Common.Interfaces.Repositories;

public interface IRepository<TId, T>
    where TId : notnull
    where T : AggregateRoot<TId>
{
    Task<T?> GetById(TId id, CancellationToken ct = default);

    void Add(T aggregate);

    void Update(T aggregate);

    void Remove(T aggregate);

    Task RemoveById(TId id, CancellationToken ct = default);
}
