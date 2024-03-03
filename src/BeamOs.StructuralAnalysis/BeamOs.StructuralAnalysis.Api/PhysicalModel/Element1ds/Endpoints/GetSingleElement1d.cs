using BeamOs.Api.PhysicalModel.Element1ds.Mappers;
using BeamOS.PhysicalModel.Contracts.Common;
using BeamOS.PhysicalModel.Contracts.Element1D;
using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using BeamOS.PhysicalModel.Domain.Element1DAggregate.ValueObjects;
using BeamOS.PhysicalModel.Infrastructure;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Api.PhysicalModel.Element1ds.Endpoints;

public class GetSingleElement1d(
    PhysicalModelDbContext dbContext,
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
        Element1D? element = await dbContext
            .Element1Ds
            .FirstAsync(n => n.Id == expectedId, cancellationToken: ct);

        if (element is null)
        {
            return null;
        }

        Element1DResponse? response = responseMapper.Map(element);

        return response;
    }
}
