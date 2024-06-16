using BeamOs.Application.Common.Interfaces.Repositories;
using BeamOs.Domain.AnalyticalResults.AnalyticalNodeAggregate.ValueObjects;
using BeamOs.Domain.AnalyticalResults.NodeResultAggregate;

namespace BeamOs.Application.AnalyticalResults.NodeResults;

public interface INodeResultRepository : IRepository<NodeResultId, NodeResult> { }
