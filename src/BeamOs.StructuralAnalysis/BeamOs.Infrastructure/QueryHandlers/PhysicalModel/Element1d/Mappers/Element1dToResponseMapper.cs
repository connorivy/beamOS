using BeamOs.Application.Common.Mappers;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Element1d.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappersJustEnums))]
internal partial class Element1dToResponseMapper
    : AbstractMapperProvidedUnits<Element1D, Element1DResponse>
{
    [Obsolete()]
    public Element1dToResponseMapper()
        : base(null) { }

    private Element1dToResponseMapper(UnitSettings unitSettings)
        : base(unitSettings) { }

    public static Element1dToResponseMapper Create(UnitSettings unitSettings)
    {
        return new(unitSettings);
    }

    public override Element1DResponse Map(Element1D source) => this.ToResponse(source);

    private partial Element1DResponse ToResponse(Element1D source);
}
