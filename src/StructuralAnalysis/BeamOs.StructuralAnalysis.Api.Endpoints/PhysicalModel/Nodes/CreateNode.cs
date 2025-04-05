using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Nodes;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "nodes")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
[BeamOsTag(BeamOsTags.AI)]
public class CreateNode(CreateNodeCommandHandler createNodeCommandHandler)
    : BeamOsModelResourceBaseEndpoint<CreateNodeCommand, CreateNodeRequest, NodeResponse>
{
    public override async Task<Result<NodeResponse>> ExecuteRequestAsync(
        CreateNodeCommand req,
        CancellationToken ct = default
    ) => await createNodeCommandHandler.ExecuteAsync(req, ct);
}
