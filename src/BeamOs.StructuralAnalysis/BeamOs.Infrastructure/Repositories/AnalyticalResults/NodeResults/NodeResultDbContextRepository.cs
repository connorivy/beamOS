using BeamOs.Application.AnalyticalResults.NodeResults;
using BeamOs.Domain.AnalyticalResults.AnalyticalNodeAggregate.ValueObjects;
using BeamOs.Domain.AnalyticalResults.NodeResultAggregate;

namespace BeamOs.Infrastructure.Repositories.AnalyticalResults.NodeResults;

internal class NodeResultDbContextRepository(BeamOsStructuralDbContext dbContext)
    : RepositoryBase<NodeResultId, NodeResult>(dbContext),
        INodeResultRepository { }
