namespace BeamOs.Common.Contracts;

public record ChatRequest
{
    public string? ConversationId { get; init; }
    public required string Message { get; init; }
    public required Guid ModelId { get; init; }
}

public record GithubModelsChatRequest
{
    public required string ApiKey { get; init; }
    public required string Message { get; init; }
}

public record GithubModelsChatCommand : IModelResourceRequest<GithubModelsChatRequest>
{
    [FromRoute]
    public Guid ModelId { get; init; }
    public GithubModelsChatRequest Body { get; init; } = null!;
    public string Message => Body.Message;
    public string ApiKey => Body.ApiKey;
}
