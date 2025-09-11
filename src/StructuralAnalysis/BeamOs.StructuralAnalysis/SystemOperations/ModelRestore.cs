using BeamOs.Common.Api;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.SystemOperations;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "restore")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class ModelRestore(
    ICommandHandler<ModelResourceRequest<DateTimeOffset>, ModelResponse> commandHandler
) : BeamOsModelResourceBaseEndpoint<DateTimeOffset, ModelResponse>
{
    public override async Task<Result<ModelResponse>> ExecuteRequestAsync(
        ModelResourceRequest<DateTimeOffset> req,
        CancellationToken ct = default
    ) => await commandHandler.ExecuteAsync(req, ct);
}
