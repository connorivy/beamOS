using BeamOS.Common.Domain.Models;

namespace BeamOS.PhysicalModel.Application.Common.Interfaces;
public interface IRepository<TId, T>
    where TId : notnull
    where T : AggregateRoot<TId>
{
    Task<T?> GetById(TId id);

    Task Add(T aggregate);
}
