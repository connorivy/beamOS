using BeamOs.Common.Api;
using BeamOs.Common.Contracts;

namespace BeamOs.Ai;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "github-models-chat")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class GithubModelsChat(GithubModelsChatCommandHandler chatCommandHandler)
    : BeamOsModelResourceBaseEndpoint<
        GithubModelsChatCommand,
        GithubModelsChatRequest,
        GithubModelsChatResponse
    >
{
    public override Task<Result<GithubModelsChatResponse>> ExecuteRequestAsync(
        GithubModelsChatCommand req,
        CancellationToken ct = default
    ) => chatCommandHandler.ExecuteAsync(req, ct);
}
