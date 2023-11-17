using BeamOS.PhysicalModel.Api.Common.Interfaces;
using BeamOS.PhysicalModel.Contracts.Element1D;
using BeamOS.PhysicalModel.Contracts.Model;
using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using BeamOS.PhysicalModel.Infrastructure;
using FastEndpoints;

namespace BeamOS.PhysicalModel.Api.Models.Endpoints;
public class GetElement1DsEndpoint(
    PhysicalModelDbContext dbContext,
    IMapper<Element1D, Element1DResponse> responseMapper) : Endpoint<GetElementsInModelRequest, List<Element1DResponse>>
{
    public override void Configure()
    {
        this.Get("models/{modelId:alpha}/element1Ds");
        this.AllowAnonymous();
    }

    public override Task<List<Element1DResponse>> ExecuteAsync(GetElementsInModelRequest req, CancellationToken ct)
    {
        if (!Guid.TryParse(req.ModelId, out Guid modelId))
        {
            throw new InvalidOperationException();
        }

        IQueryable<Element1D> element1Ds = element1Ds = dbContext.Element1Ds
                .Where(el => el.ModelId.Value == modelId);

        if (this.Query<string[]?>("id") is string[] ids)
        {
            element1Ds = element1Ds
                .Where(el => ids.Contains(el.Id.Value.ToString()));
        }

        return Task.FromResult(element1Ds.Select(responseMapper.Map).ToList());
    }
}
