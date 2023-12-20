using BeamOS.Common.Api;
using BeamOS.Common.Api.Interfaces;
using BeamOS.PhysicalModel.Api.Mappers;
using BeamOS.PhysicalModel.Contracts.Element1D;
using BeamOS.PhysicalModel.Domain.Element1DAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using BeamOS.PhysicalModel.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeamOS.PhysicalModel.Api.Element1Ds.Endpoints;

public class GetElement1ds(PhysicalModelDbContext dbContext, Element1DResponseMapper responseMapper)
    : BaseEndpoint,
        IGetEndpoint<string, List<Element1DResponse>, string[]?>
{
    public override string Route => "element1Ds";

    public async Task<List<Element1DResponse>> GetAsync(
        [FromQuery] string modelId,
        [FromQuery] string[]? element1dIds = null,
        CancellationToken ct = default
    )
    {
        ModelId modelIdTypes = new(Guid.Parse(modelId));

        if (element1dIds != null && element1dIds.Length > 0)
        {
            var element1dIdsTypes = element1dIds
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
