using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinations;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;

internal interface ILoadCombinationRepository
    : IModelResourceRepository<LoadCombinationId, LoadCombination> { }

internal sealed class InMemoryLoadCombinationRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
)
    : InMemoryModelResourceRepository<LoadCombinationId, LoadCombination>(
        inMemoryModelRepositoryStorage
    ),
        ILoadCombinationRepository { }
