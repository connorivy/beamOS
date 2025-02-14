using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Application.Common;

public interface IRepository<TId, T>
    where TId : struct
    where T : BeamOsEntity<TId>
{
    //Task<T?> GetById(TId id, CancellationToken ct = default);

    void Add(T aggregate);

    void Put(T aggregate);

    //void Update(T aggregate);

    void Remove(T aggregate);
}

public interface IModelResourceRepository<TId, T> : IRepository<TId, T>
    where TId : struct, IIntBasedId
    where T : BeamOsModelEntity<TId>
{
    public Task<List<TId>> GetIdsInModel(ModelId modelId, CancellationToken ct = default);

    public Task<T?> GetSingle(ModelId modelId, TId id);
    public Task RemoveById(ModelId modelId, TId id);
}

public interface IAnalyticalResultRepository<TId, T> : IRepository<TId, T>
    where TId : struct
    where T : BeamOsEntity<TId>
{
    Task<T?> GetSingle(ModelId modelId, ResultSetId resultSetId, TId id);
}
