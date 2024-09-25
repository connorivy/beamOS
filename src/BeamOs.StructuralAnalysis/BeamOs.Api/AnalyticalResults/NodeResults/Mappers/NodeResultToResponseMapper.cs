using BeamOs.Application.Common.Mappers;
using BeamOs.Contracts.AnalyticalResults.AnalyticalNode;
using BeamOs.Domain.AnalyticalModel.NodeResultAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.AnalyticalResults.NodeResults.Mappers;

[Mapper]
public partial class NodeResultToResponseMapper : AbstractMapper<NodeResult, NodeResultResponse>
{
    public override NodeResultResponse Map(NodeResult source) => this.ToResponse(source);

    public partial NodeResultResponse ToResponse(NodeResult source);
}
