using System.Text.Json;
using BeamOs.StructuralAnalysis.Api.Shared.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace BeamOs.StructuralAnalysis.Api;

public class UpdateNode(PatchNodeCommandHandler patchNodeCommandHandler)
    : IBaseEndpoint<PatchNodeCommand, NodeResponse>
{
    public static Func<HttpRequest, Task<PatchNodeCommand>> RequestObjectBinder =>
        RequestBinders.ModelResourceCommandBinder<PatchNodeCommand, UpdateNodeRequest>();

    public const string RouteConst = RouteConstants.ModelRoutePrefixWithTrailingSlash + "nodes";
    public static string Route => RouteConst;
    public static string EndpointName => nameof(UpdateNode);
    public static Http EndpointType => Http.PATCH;

    public async Task<Result<NodeResponse>> ExecuteRequestAsync(
        PatchNodeCommand req,
        CancellationToken ct = default
    ) => await patchNodeCommandHandler.ExecuteAsync(req, ct);

    [Function(nameof(UpdateNode))]
    public Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = RouteConst)] HttpRequest req
    ) => ((IBaseEndpoint<PatchNodeCommand, NodeResponse>)this).RunExecuteAsync<UpdateNode>(req);
}
