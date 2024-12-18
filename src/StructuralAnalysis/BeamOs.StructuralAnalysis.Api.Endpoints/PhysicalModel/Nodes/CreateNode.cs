using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Api.Endpoints.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Nodes;

//public class CreateNode(CreateNodeCommandHandler createNodeCommandHandler)
//    : IBaseEndpoint<CreateNodeCommand, NodeResponse>
//{
//    //public static Func<HttpRequest, Task<CreateNodeCommand>> RequestObjectBinder =>
//    //    RequestBinders.ModelResourceCommandBinder<CreateNodeCommand, CreateNodeRequest>();

//    public const string RouteConst = RouteConstants.ModelRoutePrefixWithTrailingSlash + "nodes";
//    public static string Route => RouteConst;

//    public const string EndpointNameConst = nameof(CreateNode);
//    public static string EndpointName => EndpointNameConst;

//    public const Http EndpointTypeConst = Http.Post;
//    public static Http EndpointType => EndpointTypeConst;
//    public static UserAuthorizationLevel RequiredAccessLevel =>
//        UserAuthorizationLevel.ModelContributor;

//    public async Task<Result<NodeResponse>> ExecuteRequestAsync(
//        CreateNodeCommand req,
//        CancellationToken ct = default
//    ) => await createNodeCommandHandler.ExecuteAsync(req, ct);

//    //[Function(EndpointNameConst)]
//    //public Task<IActionResult> Run(
//    //    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RouteConst)] HttpRequest req
//    //) => ((IBaseEndpoint<CreateNodeCommand, NodeResponse>)this).RunExecuteAsync<CreateNode>(req);
//}

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "nodes")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class CreateNode(CreateNodeCommandHandler createNodeCommandHandler)
    : BeamOsModelResourceBaseEndpoint<CreateNodeCommand, CreateNodeRequest, NodeResponse>
{
    //public static Func<HttpRequest, Task<CreateNodeCommand>> RequestObjectBinder =>
    //    RequestBinders.ModelResourceCommandBinder<CreateNodeCommand, CreateNodeRequest>();

    public override async Task<Result<NodeResponse>> ExecuteRequestAsync(
        CreateNodeCommand req,
        CancellationToken ct = default
    ) => await createNodeCommandHandler.ExecuteAsync(req, ct);

    //[Function(EndpointNameConst)]
    //public Task<IActionResult> Run(
    //    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RouteConst)] HttpRequest req
    //) => ((IBaseEndpoint<CreateNodeCommand, NodeResponse>)this).RunExecuteAsync<CreateNode>(req);
}
