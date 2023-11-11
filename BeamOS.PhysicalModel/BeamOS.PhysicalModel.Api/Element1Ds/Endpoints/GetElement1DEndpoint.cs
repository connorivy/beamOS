using BeamOS.PhysicalModel.Api.Common.Endpoints;
using BeamOS.PhysicalModel.Api.Common.Interfaces;
using BeamOS.PhysicalModel.Application.Common.Interfaces;
using BeamOS.PhysicalModel.Contracts.Element1D;
using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using BeamOS.PhysicalModel.Domain.Element1DAggregate.ValueObjects;

namespace BeamOS.PhysicalModel.Api.Element1Ds.Endpoints;
public class GetElement1DEndpoint(
    IRepository<Element1DId, Element1D> repository,
    IMapper<Element1D, Element1DResponse> responseMapper) : GetAggregateRootByGuidBasedIdEndpoint<Element1DId, Element1D, Element1DResponse>(repository, responseMapper)
{
    public override void Configure()
    {
        this.Get("element1D/{id}");
        this.AllowAnonymous();
    }
}
