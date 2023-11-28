using BeamOS.Common.Api.Interfaces;
using BeamOS.PhysicalModel.Contracts.Element1D;
using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Api.Mappers;
[Mapper]
public partial class Element1DResponseMapper : IMapper<Element1D, Element1DResponse>
{
    public Element1DResponse Map(Element1D from) => this.ToResponse(from);
    public partial Element1DResponse ToResponse(Element1D model);
}
