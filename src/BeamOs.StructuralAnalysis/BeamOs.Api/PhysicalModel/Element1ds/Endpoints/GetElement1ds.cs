using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Api.PhysicalModel.Element1ds.Mappers;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Infrastructure;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Api.PhysicalModel.Element1ds.Endpoints;

public class GetElement1ds(
    BeamOsFastEndpointOptions options,
    BeamOsStructuralDbContext dbContext,
    Element1DResponseMapper responseMapper
) : BeamOsFastEndpoint<GetElement1dsRequest, List<Element1DResponse>>(options)
{
    public override string Route => "models/{modelId}/element1Ds";

    public override Http EndpointType => Http.GET;

    public override async Task<List<Element1DResponse>> ExecuteRequestAsync(
        GetElement1dsRequest req,
        CancellationToken ct = default
    )
    {
        ModelId modelIdTypes = new(Guid.Parse(req.ModelId));

        if (req.Element1dIds != null && req.Element1dIds.Length > 0)
        {
            var element1dIdsTypes = req.Element1dIds
                .Select(id => new Element1DId(Guid.Parse(id)))
                .ToHashSet();

            return await (
                from el1d in dbContext.Element1Ds
                join m in dbContext.Models on el1d.ModelId equals m.Id
                where element1dIdsTypes.Contains(el1d.Id)
                select responseMapper.Map(el1d)
            ).ToListAsync(cancellationToken: ct);
        }
        else
        {
            return await (
                from el1d in dbContext.Element1Ds
                join m in dbContext.Models on el1d.ModelId equals m.Id
                select responseMapper.Map(el1d)
            ).ToListAsync(cancellationToken: ct);
        }
    }
}
