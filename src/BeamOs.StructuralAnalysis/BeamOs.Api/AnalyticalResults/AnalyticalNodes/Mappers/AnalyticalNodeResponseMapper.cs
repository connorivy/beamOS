using BeamOs.Api.Common;
using BeamOs.Api.Common.Interfaces;
using BeamOs.Contracts.AnalyticalResults.AnalyticalNode;
using BeamOs.Domain.AnalyticalResults.AnalyticalNodeAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.AnalyticalResults.AnalyticalNodes.Mappers;

[Mapper]
public sealed partial class AnalyticalNodeResponseMapper
    : AbstractMapper<AnalyticalNode, AnalyticalNodeResponse>
{
    public override AnalyticalNodeResponse Map(AnalyticalNode source) => this.ToResponse(source);

    public partial AnalyticalNodeResponse ToResponse(AnalyticalNode source);
}
