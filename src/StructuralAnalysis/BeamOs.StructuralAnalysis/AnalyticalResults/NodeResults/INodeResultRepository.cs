using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Application.AnalyticalResults.NodeResults;

internal interface INodeResultRepository : IAnalyticalResultRepository<NodeId, NodeResult> { }

internal class InMemoryNodeResultRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
)
    : InMemoryAnalyticalResultRepository<NodeId, NodeResult>(inMemoryModelRepositoryStorage),
        INodeResultRepository { }
