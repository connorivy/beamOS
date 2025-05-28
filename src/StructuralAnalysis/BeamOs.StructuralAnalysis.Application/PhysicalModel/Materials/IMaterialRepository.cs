using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;

public interface IMaterialRepository : IModelResourceRepository<MaterialId, Material>
{
    //public Task<List<Material>> GetMaterialsInModel(
    //    ModelId modelId,
    //    CancellationToken ct = default
    //);
}

public class InMemoryMaterialRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
)
    : InMemoryModelResourceRepository<MaterialId, Material>(inMemoryModelRepositoryStorage),
        IMaterialRepository { }
