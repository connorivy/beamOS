using BeamOS.Common.Api;
using BeamOS.Common.Api.Interfaces;
using BeamOS.PhysicalModel.Contracts.Element1D;
using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using BeamOS.PhysicalModel.Domain.Element1DAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using BeamOS.PhysicalModel.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace BeamOS.PhysicalModel.Api.Models.Endpoints;

public class GetElement1DsEndpoint(
    PhysicalModelDbContext dbContext,
    IMapper<Element1D, Element1DResponse> responseMapper)
        : BaseEndpoint, IGetEndpoint<string, List<Element1DResponse>, string[]?>
{
    public override string Route => "models/{modelId}/element1Ds";

    public Task<List<Element1DResponse>> GetAsync(
        string modelId,
        [FromQuery] string[]? element1dIds,
        CancellationToken ct)
    {
        ModelId targetModelId = new(Guid.Parse(modelId));
        IQueryable<Element1D> element1Ds = element1Ds = dbContext.Element1Ds
            .Where(el => el.ModelId == targetModelId);

        if (element1dIds is not null && element1dIds.Length > 0)
        {
            HashSet<Element1DId> element1DIdsHash = element1dIds
                .Select(s => new Element1DId(Guid.Parse(s)))
                .ToHashSet();
            element1Ds = element1Ds
                .Where(el => element1DIdsHash.Contains(el.Id));
        }

        return Task.FromResult(element1Ds.Select(responseMapper.Map).ToList());
    }
}
