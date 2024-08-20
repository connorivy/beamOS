using BeamOs.Application.AnalyticalResults.NodeResults;
using BeamOs.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.Domain.AnalyticalResults.NodeResultAggregate.ValueObjects;

namespace BeamOs.Infrastructure.Repositories.AnalyticalResults.NodeResults;

internal class NodeResultDbContextRepository(BeamOsStructuralDbContext dbContext)
    : RepositoryBase<NodeResultId, NodeResult>(dbContext),
        INodeResultRepository { }
