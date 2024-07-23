using BeamOs.Application.Common.Mappers;
using BeamOs.Contracts.AnalyticalResults.AnalyticalNode;
using BeamOs.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Infrastructure.Data.Models;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Infrastructure.QueryHandlers.AnalyticalResultReadModels.NodeResultReadModels.Mappers;

[Mapper]
internal partial class NodeResultReadModelToResponse
    : AbstractMapperProvidedUnits<NodeResultReadModel, NodeResultResponse>
{
    [Obsolete()]
    public NodeResultReadModelToResponse()
        : base(null) { }

    private NodeResultReadModelToResponse(UnitSettings unitSettings)
        : base(unitSettings) { }

    public static NodeResultReadModelToResponse Create(UnitSettings unitSettings)
    {
        return new(unitSettings);
    }

    public override NodeResultResponse Map(NodeResultReadModel source) => this.ToResponse(source);

    public partial NodeResultResponse ToResponse(NodeResultReadModel nodeResultReadModel);
}

[Mapper]
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
