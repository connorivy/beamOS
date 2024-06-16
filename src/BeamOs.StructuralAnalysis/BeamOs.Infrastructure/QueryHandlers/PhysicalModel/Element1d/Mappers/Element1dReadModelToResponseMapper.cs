using BeamOs.Application.Common.Mappers;
using BeamOs.Application.PhysicalModel.Element1dAggregate.Interfaces;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Infrastructure.Data.Models;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Element1d.Mappers;

[Mapper]
internal partial class Element1dReadModelToResponseMapper
    : AbstractMapper<Element1dReadModel, Element1DResponse>
{
    public override Element1DResponse Map(Element1dReadModel source) => this.ToResponse(source);

    private partial Element1DResponse ToResponse(Element1dReadModel source);
}
