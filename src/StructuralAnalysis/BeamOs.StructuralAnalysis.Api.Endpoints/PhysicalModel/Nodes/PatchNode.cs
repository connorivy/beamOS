using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Nodes;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "nodes")]
[BeamOsEndpointType(Http.Patch)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class UpdateNode(PatchNodeCommandHandler patchNodeCommandHandler)
    : BeamOsModelResourceBaseEndpoint<PatchNodeCommand, UpdateNodeRequest, NodeResponse>
{
    public override async Task<Result<NodeResponse>> ExecuteRequestAsync(
        PatchNodeCommand req,
        CancellationToken ct = default
    ) => await patchNodeCommandHandler.ExecuteAsync(req, ct);
}
