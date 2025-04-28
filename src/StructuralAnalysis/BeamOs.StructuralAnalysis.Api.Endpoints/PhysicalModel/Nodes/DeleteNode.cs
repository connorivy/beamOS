using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Api.Endpoints.OpenSees;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Nodes;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "nodes/{id}")]
[BeamOsEndpointType(Http.Delete)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
[BeamOsTag(BeamOsTags.AI)]
public class DeleteNode(DeleteNodeCommandHandler deleteNodeCommandHandler)
    : BeamOsModelResourceQueryBaseEndpoint<ModelEntityResponse>
{
    public override async Task<Result<ModelEntityResponse>> ExecuteRequestAsync(
        ModelEntityRequest req,
        CancellationToken ct = default
    ) => await deleteNodeCommandHandler.ExecuteAsync(req, ct);
}
