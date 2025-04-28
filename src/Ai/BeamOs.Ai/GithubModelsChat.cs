using BeamOs.Common.Api;
using BeamOs.Common.Contracts;

namespace BeamOs.Ai;

[BeamOsRoute("github-models-chat")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Authenticated)]
public class GithubModelsChat(GithubModelsChatCommandHandler chatCommandHandler)
    : BeamOsFromBodyResultBaseEndpoint<GithubModelsChatRequest, string>
{
    public override Task<Result<string>> ExecuteRequestAsync(
        GithubModelsChatRequest req,
        CancellationToken ct = default
    ) => chatCommandHandler.ExecuteAsync(req, ct);
}
