using BeamOs.StructuralAnalysis.Application.AnalyticalResults.NodeResults;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Infrastructure.Common;

namespace BeamOs.StructuralAnalysis.Infrastructure.AnalyticalResults.NodeResults;

internal sealed class NodeResultRepository(StructuralAnalysisDbContext dbContext)
    : AnalyticalResultRepositoryBase<NodeId, NodeResult>(dbContext),
        INodeResultRepository { }
