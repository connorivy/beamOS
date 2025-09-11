using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Application.AnalyticalResults.NodeResults;

public interface INodeResultRepository : IAnalyticalResultRepository<NodeId, NodeResult> { }

public class InMemoryNodeResultRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
)
    : InMemoryAnalyticalResultRepository<NodeId, NodeResult>(inMemoryModelRepositoryStorage),
        INodeResultRepository { }
