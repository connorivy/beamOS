using BeamOs.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.Domain.AnalyticalResults.NodeResultAggregate.ValueObjects;

namespace BeamOs.Application.AnalyticalResults.NodeResults;

public interface INodeResultRepository : IRepository<NodeResultId, NodeResult> { }
