using BeamOs.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.Domain.PhysicalModel.MaterialAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;

namespace BeamOs.Application.PhysicalModel.Materials;

public interface IMaterialRepository : IRepository<MaterialId, Material>
{
    public Task<List<Material>> GetMaterialsInModel(
        ModelId modelId,
        CancellationToken ct = default
    );
}
