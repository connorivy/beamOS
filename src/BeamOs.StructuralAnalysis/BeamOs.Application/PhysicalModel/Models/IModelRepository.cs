using BeamOs.Application.Common.Interfaces.Repositories;
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
}
