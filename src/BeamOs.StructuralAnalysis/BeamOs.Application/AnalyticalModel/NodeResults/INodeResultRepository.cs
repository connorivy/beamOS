using BeamOs.Domain.AnalyticalModel.NodeResultAggregate;
using BeamOs.Domain.AnalyticalModel.NodeResultAggregate.ValueObjects;

namespace BeamOs.Application.AnalyticalModel.NodeResults;

public interface INodeResultRepository : IRepository<NodeResultId, NodeResult> { }
