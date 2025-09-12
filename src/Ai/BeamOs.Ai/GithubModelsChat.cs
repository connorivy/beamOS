using BeamOs.Common.Api;
using BeamOs.Common.Contracts;

namespace BeamOs.Ai;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "github-models-chat")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class GithubModelsChat(GithubModelsChatCommandHandler chatCommandHandler)
    : BeamOsModelResourceBaseEndpoint<GithubModelsChatRequest, GithubModelsChatResponse>
{
    public override Task<Result<GithubModelsChatResponse>> ExecuteRequestAsync(
        ModelResourceRequest<GithubModelsChatRequest> req,
        CancellationToken ct = default
    ) => chatCommandHandler.ExecuteAsync(req, ct);
}
