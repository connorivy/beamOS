using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.Element1ds.Mappers;

[Mapper]
public partial class Element1DResponseMapper : IMapper<Element1D, Element1DResponse>
{
    public Element1DResponse Map(Element1D from) => this.ToResponse(from);

    public partial Element1DResponse ToResponse(Element1D model);
}
