using BeamOs.Application.Common.Mappers;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Contracts.AnalyticalResults.AnalyticalNode;
using BeamOs.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.Domain.Common.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Infrastructure.QueryHandlers.AnalyticalResultReadModels.NodeResultReadModels.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappersJustEnums))]
internal partial class NodeResultToResponse
    : AbstractMapperProvidedUnits<NodeResult, NodeResultResponse>
{
    [Obsolete()]
    public NodeResultToResponse()
        : base(null) { }

    private NodeResultToResponse(UnitSettings unitSettings)
        : base(unitSettings) { }

    public static NodeResultToResponse Create(UnitSettings unitSettings)
    {
        return new(unitSettings);
    }

    public override NodeResultResponse Map(NodeResult source) => this.ToResponse(source);

    public partial NodeResultResponse ToResponse(NodeResult nodeResult);
}
