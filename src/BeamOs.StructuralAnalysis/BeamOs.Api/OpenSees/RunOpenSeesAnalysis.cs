using BeamOS.Api.Common;
using BeamOs.Application.OpenSees;
using BeamOs.Contracts.Common;
using FastEndpoints;

namespace BeamOs.Api.OpenSees;

public class RunOpenSeesAnalysis(
    RunOpenSeesAnalysisCommandHandler runOpenSeesAnalysisCommandHandler
) : BeamOsFastEndpoint<ModelIdRequest, bool>
{
    public override string Route => "models/{modelId}/analyze/opensees";

    public override Http EndpointType => Http.GET;

    public override async Task<bool> ExecuteRequestAsync(ModelIdRequest req, CancellationToken ct)
    {
        return await runOpenSeesAnalysisCommandHandler.ExecuteAsync(req, ct);
    }
}
