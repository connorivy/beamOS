using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;

internal interface ILoadCaseRepository : IModelResourceRepository<LoadCaseId, LoadCase> { }

internal sealed class InMemoryLoadCaseRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
)
    : InMemoryModelResourceRepository<LoadCaseId, LoadCase>(inMemoryModelRepositoryStorage),
        ILoadCaseRepository { }
