using BeamOs.Application.Common.Interfaces.Repositories;
using BeamOs.Domain.AnalyticalResults.AnalyticalModelAggregate.ValueObjects;
using BeamOs.Domain.AnalyticalResults.ModelResultAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;

namespace BeamOs.Application.AnalyticalResults.ModelResults;

public interface IModelResultRepository : IRepository<ModelResultId, ModelResult>
{
    Task<ModelResult?> GetByModelId(ModelId modelId, CancellationToken ct = default);
}
