using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;

namespace BeamOs.Application.PhysicalModel.Materials;

public interface IMaterialRepository : IRepository<MaterialId, Material>
{
    //public Task<List<Material>> GetMaterialsInModel(
    //    ModelId modelId,
    //    CancellationToken ct = default
    //);
}
