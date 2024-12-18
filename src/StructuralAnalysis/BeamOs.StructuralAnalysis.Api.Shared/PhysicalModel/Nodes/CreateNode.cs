using BeamOs.StructuralAnalysis.Api.Shared.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace BeamOs.StructuralAnalysis.Api;

public class CreateNode(CreateNodeCommandHandler createNodeCommandHandler)
    : IBaseEndpoint<CreateNodeCommand, NodeResponse>
{
    public static Func<HttpRequest, Task<CreateNodeCommand>> RequestObjectBinder =>
        RequestBinders.ModelResourceCommandBinder<CreateNodeCommand, CreateNodeRequest>();

    public const string RouteConst = RouteConstants.ModelRoutePrefixWithTrailingSlash + "nodes";
    public static string Route => RouteConst;

    public const string EndpointNameConst = nameof(CreateNode);
    public static string EndpointName => EndpointNameConst;

    public const Http EndpointTypeConst = Http.POST;
    public static Http EndpointType => EndpointTypeConst;
    public static UserAuthorizationLevel RequiredAccessLevel =>
        UserAuthorizationLevel.ModelContributor;

    public async Task<Result<NodeResponse>> ExecuteRequestAsync(
        CreateNodeCommand req,
        CancellationToken ct = default
    ) => await createNodeCommandHandler.ExecuteAsync(req, ct);

    [Function(EndpointNameConst)]
    public Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RouteConst)] HttpRequest req
    ) => ((IBaseEndpoint<CreateNodeCommand, NodeResponse>)this).RunExecuteAsync<CreateNode>(req);
}
