using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.AnalyticalResults.Model;
using BeamOs.Contracts.Common;
using FastEndpoints;

namespace BeamOs.Api.AnalyticalResults.ModelResults.Endpoints;

public class GetModelResults(
    BeamOsFastEndpointOptions options,
    IQueryHandler<ModelIdRequest, ModelResultResponse> modelResultResponseQueryHandler
) : BeamOsFastEndpoint<ModelIdRequest, ModelResultResponse>(options)
{
    public override Http EndpointType => Http.GET;
    public override string Route => "models/{modelId}/results";

    public override async Task<ModelResultResponse?> ExecuteRequestAsync(
        ModelIdRequest req,
        CancellationToken ct
    )
    {
        return await modelResultResponseQueryHandler.ExecuteAsync(req, ct);
    }
}
