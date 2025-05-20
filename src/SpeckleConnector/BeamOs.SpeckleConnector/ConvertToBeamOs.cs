using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

namespace BeamOs.SpeckleConnector;

[BeamOsRoute("speckle-receive")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Authenticated)]
public class SpeckleRecieveOperation(BeamOsSpeckleReceiveOperation2 beamOsSpeckleReceiveOperation)
    : BeamOsModelResourceBaseEndpoint<
        ModelResourceRequest<SpeckleReceiveParameters>,
        SpeckleReceiveParameters,
        ModelProposalResponse
    >
{
    public override async Task<Result<ModelProposalResponse>> ExecuteRequestAsync(
        ModelResourceRequest<SpeckleReceiveParameters> req,
        CancellationToken ct = default
    ) => await beamOsSpeckleReceiveOperation.ExecuteAsync(req, ct).ConfigureAwait(false);
}
