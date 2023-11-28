using BeamOS.Common.Domain.Models;

namespace BeamOS.Common.Application.Interfaces;
public interface IRepository<TId, T>
    where TId : notnull
    where T : AggregateRoot<TId>
{
    Task<T?> GetById(TId id);

    Task Add(T aggregate);
}
