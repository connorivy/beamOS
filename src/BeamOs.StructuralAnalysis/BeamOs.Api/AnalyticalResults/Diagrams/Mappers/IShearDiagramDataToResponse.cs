using BeamOs.Application.Common.Mappers;
using BeamOs.Contracts.AnalyticalResults.Diagrams;
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
