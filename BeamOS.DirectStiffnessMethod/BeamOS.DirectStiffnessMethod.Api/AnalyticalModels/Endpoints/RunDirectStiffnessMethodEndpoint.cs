using BeamOS.Common.Api;
using BeamOS.Common.Api.Interfaces;

namespace BeamOS.DirectStiffnessMethod.Api.AnalyticalModels.Endpoints;

public class RunDirectStiffnessMethodEndpoint(PhysicalModelApiClient physicalModelApi)
    : BaseEndpoint, IPostEndpoint<string, string>
{
    public override string Route => "/analytical-model/{id}";

    public async Task<string> PostAsync(string id, CancellationToken ct)
    {
        var x = await physicalModelApi.GetModelResponse(id);

        return null;
    }
}
