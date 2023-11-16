using BeamOS.PhysicalModel.Api.Common.Interfaces;
using BeamOS.PhysicalModel.Api.Data;
using BeamOS.PhysicalModel.Contracts.Common;
using BeamOS.PhysicalModel.Contracts.Element1D;
using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using FastEndpoints;

namespace BeamOS.PhysicalModel.Api.Element1Ds.Endpoints;
public class GetSingleElement1DEndpoint(
    PhysicalModelDbContext dbContext,
    IMapper<Element1D, Element1DResponse> responseMapper) : Endpoint<IdRequest, Element1DResponse?>
{
    public override void Configure()
    {
        this.Get("element1Ds/{id}");
        this.AllowAnonymous();
    }

    public override Task<Element1DResponse?> ExecuteAsync(IdRequest req, CancellationToken ct)
    {
        Element1D? element = dbContext.Element1Ds
            .Where(e => e.Id.Value == Guid.Parse(req.Id))
            .FirstOrDefault();

        if (element is null)
        {
            return Task.FromResult<Element1DResponse?>(null);
        }

        Element1DResponse? response = responseMapper.Map(element);

        return Task.FromResult<Element1DResponse?>(response);
    }
}
