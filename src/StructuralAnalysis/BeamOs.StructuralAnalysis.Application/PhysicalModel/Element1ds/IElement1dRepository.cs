using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;

public interface IElement1dRepository : IModelResourceRepository<Element1dId, Element1d>
{
    //public Task<List<Element1D>> GetElement1dsInModel(
    //    ModelId modelId,
    //    CancellationToken ct = default
    //);
}
