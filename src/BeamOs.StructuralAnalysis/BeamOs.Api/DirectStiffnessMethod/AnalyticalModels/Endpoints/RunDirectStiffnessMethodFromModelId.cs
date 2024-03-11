using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Api.PhysicalModel.Models.Endpoints;
using BeamOs.Contracts.AnalyticalResults.Model;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Model;
using FastEndpoints;

namespace BeamOs.Api.DirectStiffnessMethod.AnalyticalModels.Endpoints;

public class RunDirectStiffnessMethodFromModelId(
    BeamOsFastEndpointOptions options,
    GetModelHydrated getModelHydratedEndpoint,
    RunDirectStiffnessMethod runDirectStiffnessMethodEndpoint
) : BeamOsFastEndpoint<IdRequestFromPath, AnalyticalModelResponse2>(options)
{
    public override string Route => "/direct-stiffness-method/{id}";

    public override Http EndpointType => Http.GET;

    public override async Task<AnalyticalModelResponse2> ExecuteAsync(
        IdRequestFromPath req,
        CancellationToken ct
    )
    {
        ModelResponseHydrated responseHydrated = await getModelHydratedEndpoint.ExecuteAsync(
            req,
            ct
        );
        return await runDirectStiffnessMethodEndpoint.ExecuteAsync(responseHydrated, ct);
    }
}
