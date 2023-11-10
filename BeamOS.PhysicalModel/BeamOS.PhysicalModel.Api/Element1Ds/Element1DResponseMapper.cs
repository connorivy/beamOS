using BeamOS.PhysicalModel.Contracts.Element1D;
using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Api.Mappers;
[Mapper]
public static partial class Element1DResponseMapper
{
    public static partial Element1DResponse ToResponse(this Element1D model);
}
