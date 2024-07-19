using BeamOs.Application.Common.Mappers;
using BeamOs.Contracts.AnalyticalResults.Diagrams;
using BeamOs.Domain.Diagrams.MomentDiagramAggregate;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.AnalyticalResults.Diagrams.Mappers;

[Mapper]
public partial class ShearDiagramDataToResponse
    : AbstractMapper<ShearForceDiagram, ShearDiagramResponse>
{
    public override ShearDiagramResponse Map(ShearForceDiagram source) => this.ToResponse(source);

    public partial ShearDiagramResponse ToResponse(ShearForceDiagram source);
}

[Mapper]
public partial class MomentDiagramDataToResponse
    : AbstractMapper<MomentDiagram, MomentDiagramResponse>
{
    public override MomentDiagramResponse Map(MomentDiagram source) => this.ToResponse(source);

    public partial MomentDiagramResponse ToResponse(MomentDiagram source);
}
