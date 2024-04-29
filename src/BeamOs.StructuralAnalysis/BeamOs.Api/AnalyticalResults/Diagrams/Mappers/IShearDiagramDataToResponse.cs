using BeamOs.Application.AnalyticalResults.Diagrams.ShearDiagrams.Interfaces;
using BeamOs.Application.Common.Models;
using BeamOs.Contracts.AnalyticalResults.Diagrams;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.AnalyticalResults.Diagrams.Mappers;

[Mapper]
public partial class IShearDiagramDataToResponse
    : AbstractMapper<IShearDiagramData, ShearDiagramResponse>
{
    public override ShearDiagramResponse Map(IShearDiagramData source) => this.ToResponse(source);

    public partial ShearDiagramResponse ToResponse(IShearDiagramData source);
}
