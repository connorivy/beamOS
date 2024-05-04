using BeamOs.Application.Common.Interfaces.Repositories;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;

namespace BeamOs.Application.PhysicalModel.Element1dAggregate;

public interface IElement1dRepository : IRepository<Element1DId, Element1D>
{
    public Task<List<Element1D>> GetElement1dsInModel(
        ModelId modelId,
        CancellationToken ct = default
    );
}
