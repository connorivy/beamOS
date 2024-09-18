using BeamOS.Api.Common;
using BeamOs.Application.OpenSees;
using BeamOs.Contracts.Common;
using FastEndpoints;

namespace BeamOs.Api.OpenSees;

public class GetTclFromModel(GetTclFromModelCommandHandler getTclFromModelCommandHandler)
    : BeamOsFastEndpoint<ModelIdRequest, string>
{
    public override string Route => "models/{modelId}/opensees/tcl";

    public override Http EndpointType => Http.GET;

    public override async Task<string> ExecuteRequestAsync(ModelIdRequest req, CancellationToken ct)
    {
        return await getTclFromModelCommandHandler.ExecuteAsync(req, ct);
    }
}
