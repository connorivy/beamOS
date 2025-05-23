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

    public void Add(T aggregate);

    public void Put(T aggregate);

    //void Update(T aggregate);

    public void Remove(T aggregate);

    public void ClearChangeTracker();
}

public interface IModelResourceRepository<TId, T> : IRepository<TId, T>
    where TId : struct, IIntBasedId
    where T : BeamOsModelEntity<TId>
{
    public Task<List<TId>> GetIdsInModel(ModelId modelId, CancellationToken ct = default);
    public Task<List<T>> GetMany(ModelId modelId, IList<TId>? ids, CancellationToken ct = default);
    public Task<T?> GetSingle(ModelId modelId, TId id, CancellationToken ct = default);
    public Task RemoveById(ModelId modelId, TId id, CancellationToken ct = default);
    public Task ReloadEntity(T entity, CancellationToken ct = default);
}

public interface IAnalyticalResultRepository<TId, T> : IRepository<TId, T>
    where TId : struct
    where T : BeamOsEntity<TId>
{
    Task<T?> GetSingle(ModelId modelId, ResultSetId resultSetId, TId id);
}

public interface IProposalRepository<TId, T> : IRepository<TId, T>
    where TId : struct
    where T : BeamOsEntity<TId>
{
    public Task<T?> GetSingle(ModelId modelId, ModelProposalId modelProposalId, TId id);
}
