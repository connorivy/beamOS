using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinations;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;

public interface ILoadCombinationRepository
    : IModelResourceRepository<LoadCombinationId, LoadCombination> { }
