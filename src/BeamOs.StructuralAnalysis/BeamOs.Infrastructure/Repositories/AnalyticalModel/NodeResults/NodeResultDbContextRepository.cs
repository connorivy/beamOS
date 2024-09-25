using BeamOs.Application.AnalyticalModel.NodeResults;
using BeamOs.Domain.AnalyticalModel.NodeResultAggregate;
using BeamOs.Domain.AnalyticalModel.NodeResultAggregate.ValueObjects;

namespace BeamOs.Infrastructure.Repositories.AnalyticalModel.NodeResults;

internal class NodeResultDbContextRepository(BeamOsStructuralDbContext dbContext)
    : RepositoryBase<NodeResultId, NodeResult>(dbContext),
        INodeResultRepository { }
