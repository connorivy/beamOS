using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Application.Common;

public interface IRepository<TId, T>
    where TId : struct
    where T : BeamOsEntity<TId>
{
    //Task<T?> GetById(TId id, CancellationToken ct = default);

    void Add(T aggregate);

    //void Update(T aggregate);

    void Remove(T aggregate);

    //Task RemoveById(TId id, CancellationToken ct = default);
}
