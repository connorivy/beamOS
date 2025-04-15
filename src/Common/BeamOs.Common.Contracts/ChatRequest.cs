namespace BeamOs.Common.Contracts;

public record ChatRequest
{
    public string? ConversationId { get; init; }
    public required string Message { get; init; }
    public required Guid ModelId { get; init; }
}

public record GithubModelsChatRequest : ChatRequest
{
    public required string ApiKey { get; init; }
}
