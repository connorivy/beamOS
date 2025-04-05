using BeamOs.Common.Api;
using BeamOs.Common.Contracts;

namespace BeamOs.Ai;

[BeamOsRoute("chat")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Authenticated)]
public class Chat(ChatCommandHandler chatCommandHandler)
    : BeamOsActualBaseEndpoint<ChatRequest, IAsyncEnumerable<string>>
{
    public override IAsyncEnumerable<string> ExecuteRequestAsync(
        ChatRequest req,
        CancellationToken ct = default
    ) => chatCommandHandler.ExecuteChatAsync(req, ct);
}

public record ChatRequest
{
    public string? ConversationId { get; set; }
    public required string Message { get; set; }
}
