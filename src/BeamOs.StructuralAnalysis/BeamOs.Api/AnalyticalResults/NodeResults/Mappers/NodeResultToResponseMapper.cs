using BeamOs.Application.Common.Mappers;
using BeamOs.Contracts.AnalyticalResults.AnalyticalNode;
using BeamOs.Domain.AnalyticalResults.NodeResultAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.AnalyticalResults.NodeResults.Mappers;

[Mapper]
public partial class NodeResultToResponseMapper : AbstractMapper<NodeResult, AnalyticalNodeResponse>
{
    public override AnalyticalNodeResponse Map(NodeResult source) => this.ToResponse(source);

    public partial AnalyticalNodeResponse ToResponse(NodeResult source);
}
