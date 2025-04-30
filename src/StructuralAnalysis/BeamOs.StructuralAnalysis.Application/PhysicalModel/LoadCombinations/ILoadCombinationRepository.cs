using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinationAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;

public interface ILoadCombinationRepository : IModelResourceRepository<LoadCombinationId, LoadCombination>
{
}
