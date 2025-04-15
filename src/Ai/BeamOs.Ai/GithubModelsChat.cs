using BeamOs.Common.Api;
using BeamOs.Common.Contracts;

namespace BeamOs.Ai;

[BeamOsRoute("github-models-chat")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Authenticated)]
public class GithubModelsChat(GithubModelsChatCommandHandler chatCommandHandler)
    : BeamOsFromBodyBaseEndpoint<GithubModelsChatRequest, IAsyncEnumerable<string>>
{
    public override IAsyncEnumerable<string> ExecuteRequestAsync(
        GithubModelsChatRequest req,
        CancellationToken ct = default
    ) => chatCommandHandler.ExecuteAsync(req, ct);
}
