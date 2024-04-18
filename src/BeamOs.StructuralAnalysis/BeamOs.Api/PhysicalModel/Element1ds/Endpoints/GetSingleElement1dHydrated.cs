using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.PhysicalModel.Element1dAggregate.Queries;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Element1d;
using FastEndpoints;

namespace BeamOs.Api.PhysicalModel.Element1ds.Endpoints;

public class GetSingleElement1dHydrated(
    BeamOsFastEndpointOptions options,
    IQueryHandler<
        GetElement1dHydratedByIdQuery,
        Element1dResponseHydrated
    > getElement1dHydratedByIdQueryHandler
) : BeamOsFastEndpoint<IdRequest, Element1dResponseHydrated?>(options)
{
    public override Http EndpointType => Http.GET;

    public override string Route => "element1ds/{id}/" + CommonApiConstants.HYDRATED_ROUTE;

    public override async Task<Element1dResponseHydrated?> ExecuteAsync(
        IdRequest req,
        CancellationToken ct
    )
    {
        GetElement1dHydratedByIdQuery query = new(Guid.Parse(req.Id));
        return await getElement1dHydratedByIdQueryHandler.ExecuteAsync(query, ct);
    }
}
