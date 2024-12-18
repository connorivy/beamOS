using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Api.Endpoints.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Nodes;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "nodes")]
[BeamOsEndpointType(Http.Patch)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class UpdateNode(PatchNodeCommandHandler patchNodeCommandHandler)
    : BeamOsModelResourceBaseEndpoint<PatchNodeCommand, UpdateNodeRequest, NodeResponse>
{
    //public static Func<HttpRequest, Task<PatchNodeCommand>> RequestObjectBinder =>
    //    RequestBinders.ModelResourceCommandBinder<PatchNodeCommand, UpdateNodeRequest>();

    //public const string RouteConst = RouteConstants.ModelRoutePrefixWithTrailingSlash + "nodes";
    //public override string Route => RouteConst;
    //public override string EndpointName => nameof(UpdateNode);
    //public override Http EndpointType => Http.Patch;
    //public override UserAuthorizationLevel RequiredAccessLevel =>
    //    UserAuthorizationLevel.ModelContributor;

    public override async Task<Result<NodeResponse>> ExecuteRequestAsync(
        PatchNodeCommand req,
        CancellationToken ct = default
    ) => await patchNodeCommandHandler.ExecuteAsync(req, ct);

    //[Function(nameof(UpdateNode))]
    //public Task<IActionResult> Run(
    //    [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = RouteConst)] HttpRequest req
    //) => ((IBaseEndpoint<PatchNodeCommand, NodeResponse>)this).RunExecuteAsync<UpdateNode>(req);
}
