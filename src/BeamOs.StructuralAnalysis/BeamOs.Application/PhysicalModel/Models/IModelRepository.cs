using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;

namespace BeamOs.Application.PhysicalModel.Models;

public interface IModelRepository : IRepository<ModelId, Model>
{
    Task<Model?> GetById(
        ModelId id,
        CancellationToken ct = default,
        params string[] propertiesToLoad
    );

    Task<UnitSettings> GetUnits(ModelId id, CancellationToken ct);
}
