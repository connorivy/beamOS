using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.Common.Queries;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Model;
using FastEndpoints;

namespace BeamOs.Api.PhysicalModel.Models.Endpoints;

public class GetModel(
    BeamOsFastEndpointOptions options,
    IQueryHandler<GetResourceByIdWithPropertiesQuery, ModelResponse> getResourceByIdQueryHandler
) : BeamOsFastEndpoint<IdRequestWithProperties, ModelResponse>(options)
{
    public override string Route => "models/{id}";

    public override Http EndpointType => Http.GET;

    public override async Task<ModelResponse?> ExecuteAsync(
        IdRequestWithProperties req,
        CancellationToken ct
    )
    {
        GetResourceByIdWithPropertiesQuery query =
            new(Guid.Parse(req.Id), req.Properties?.ToHashSet());
        return await getResourceByIdQueryHandler.ExecuteAsync(query, ct);
    }
}
