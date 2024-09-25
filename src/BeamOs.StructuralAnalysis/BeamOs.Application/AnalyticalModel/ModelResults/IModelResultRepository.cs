using BeamOs.Domain.AnalyticalModel.AnalyticalResultsAggregate;
using BeamOs.Domain.AnalyticalModel.AnalyticalResultsAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;

namespace BeamOs.Application.AnalyticalModel.ModelResults;

public interface IModelResultRepository : IRepository<AnalyticalResultsId, AnalyticalResults>
{
    Task<AnalyticalResults?> GetByModelId(ModelId modelId, CancellationToken ct = default);
}
