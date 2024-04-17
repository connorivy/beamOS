using BeamOs.Api.PhysicalModel.Element1ds.Mappers;
using BeamOs.Application.Common.Interfaces;
using BeamOs.Contracts.PhysicalModel.Common;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;
using BeamOs.Infrastructure;
using FastEndpoints;

namespace BeamOs.Api.PhysicalModel.Element1ds.Endpoints;

public class GetSingleElement1d(
    IRepository<Element1DId, Element1D> element1dRepository,
    Element1DResponseMapper responseMapper
) : Endpoint<IdRequest, Element1DResponse?>
{
    public override void Configure()
    {
        this.Get("element1Ds/{id}");
        this.AllowAnonymous();
    }

    public override async Task<Element1DResponse?> ExecuteAsync(IdRequest req, CancellationToken ct)
    {
        Element1DId expectedId = new(Guid.Parse(req.Id));
        Element1D? element = await element1dRepository.GetById(expectedId);
        //Element1D? element = await dbContext
        //    .Element1Ds
        //    .FirstAsync(n => n.Id == expectedId, cancellationToken: ct);

        if (element is null)
        {
            return null;
        }

        Element1DResponse? response = responseMapper.Map(element);

        return response;
    }
}
