using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Api.PhysicalModel.Models.Endpoints;
using BeamOs.Contracts.AnalyticalResults.Model;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Model;
using FastEndpoints;

namespace BeamOs.Api.DirectStiffnessMethod.AnalyticalModels.Endpoints;

public class RunDirectStiffnessMethodFromModelId2(
    BeamOsFastEndpointOptions options,
    GetModelHydrated getModelHydratedEndpoint,
    RunDirectStiffnessMethod runDirectStiffnessMethodEndpoint
) : BeamOsFastEndpoint<IdRequestFromPath, AnalyticalModelResponse>(options)
{
    public override string Route => "/direct-stiffness-method/v2/{id}";

    public override Http EndpointType => Http.GET;

    public override async Task<AnalyticalModelResponse> ExecuteAsync(
        IdRequestFromPath req,
        CancellationToken ct
    )
    {
        GetModelHydratedRequest modelHydratedRequest = new(req.Id, null);

        ModelResponseHydrated responseHydrated = await getModelHydratedEndpoint.ExecuteAsync(
            modelHydratedRequest,
            ct
        );
        return await runDirectStiffnessMethodEndpoint.ExecuteAsync(responseHydrated, ct);
    }
}
