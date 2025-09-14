using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads;

internal interface IMomentLoadRepository : IModelResourceRepository<MomentLoadId, MomentLoad> { }

internal sealed class InMemoryMomentLoadRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
)
    : InMemoryModelResourceRepository<MomentLoadId, MomentLoad>(inMemoryModelRepositoryStorage),
        IMomentLoadRepository { }
