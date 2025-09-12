using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads;

internal interface IPointLoadRepository : IModelResourceRepository<PointLoadId, PointLoad> { }

internal sealed class InMemoryPointLoadRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
)
    : InMemoryModelResourceRepository<PointLoadId, PointLoad>(inMemoryModelRepositoryStorage),
        IPointLoadRepository { }
