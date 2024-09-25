using BeamOS.Api.Common;
using BeamOs.Application.AnalyticalModel.ModelResults;
using BeamOs.Contracts.Common;
using FastEndpoints;

namespace BeamOs.Api.AnalyticalResults.ModelResults.Endpoints;

public class DeleteModelResults(
    DeleteAnalyticalModelByModelIdCommandHandler deleteAnalyticalModelByModelIdCommandHandler
) : BeamOsFastEndpoint<ModelIdRequest, bool>
{
    public override Http EndpointType => Http.DELETE;
    public override string Route => "models/{modelId}/results";

    public override async Task<bool> ExecuteRequestAsync(ModelIdRequest req, CancellationToken ct)
    {
        return await deleteAnalyticalModelByModelIdCommandHandler.ExecuteAsync(req);
    }
}
