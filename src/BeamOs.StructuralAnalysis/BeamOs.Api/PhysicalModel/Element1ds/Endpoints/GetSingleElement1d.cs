using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.Common.Queries;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Element1d;
using FastEndpoints;

namespace BeamOs.Api.PhysicalModel.Element1ds.Endpoints;

public class GetSingleElement1d(
    BeamOsFastEndpointOptions options,
    IQueryHandler<
        GetResourceByIdQuery,
        Element1DResponse
    > getResourceToElement1dResponseQueryHandler
) : BeamOsFastEndpoint<IdRequest, Element1DResponse?>(options)
{
    public override Http EndpointType => Http.GET;
    public override string Route => "element1Ds/{id}";

    public override async Task<Element1DResponse?> ExecuteAsync(IdRequest req, CancellationToken ct)
    {
        GetResourceByIdQuery query = new(Guid.Parse(req.Id));

        return await getResourceToElement1dResponseQueryHandler.ExecuteAsync(query, ct);
    }
}
