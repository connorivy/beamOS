using BeamOs.Application.Common.Mappers;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Contracts.AnalyticalResults.Diagrams;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.Diagrams.MomentDiagramAggregate;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.AnalyticalResults.Diagrams.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
public partial class ShearDiagramDataToResponse
    : AbstractMapper<ShearForceDiagram, ShearDiagramResponse>
{
    public override ShearDiagramResponse Map(ShearForceDiagram source) => this.ToResponse(source);

    public partial ShearDiagramResponse ToResponse(ShearForceDiagram source);
}

[Mapper]
public partial class MomentDiagramDataToResponse
    : AbstractMapperProvidedUnits<MomentDiagram, MomentDiagramResponse>
{
    [Obsolete("This is just here to make DI registration work. I'm too lazy to change it.", true)]
    public MomentDiagramDataToResponse()
        : base(null) { }

    private MomentDiagramDataToResponse(UnitSettings unitSettings)
        : base(unitSettings) { }

    public static MomentDiagramDataToResponse Create(UnitSettings unitSettings)
    {
        return new(unitSettings);
    }

    public override MomentDiagramResponse Map(MomentDiagram source) => this.ToResponse(source);

    public partial MomentDiagramResponse ToResponse(MomentDiagram source);
}
