namespace BeamOs.Identity;

public record CreateApiTokenRequest
{
    public required string Name { get; init; }
    public required List<string> Scopes { get; init; }
}

public record ApiTokenResponse
{
    public string Name { get; init; }
    public DateTimeOffset CreatedOn { get; init; }
    public List<string> Scopes { get; init; }
    public string Value { get; init; }
}

public record ApiUsageResponse
{
    public required int TotalCalls { get; init; }
    public required long TotalDurationMs { get; init; }
    public required List<UsageBreakdownResponse> Breakdown { get; init; }
}

public record UsageBreakdownResponse
{
    public required string ActorName { get; init; }
    public required int NumCalls { get; init; }
    public required long TotalDurationMs { get; init; }
    public bool IsToken { get; init; }

    public double SharePercentage(long totalDurationMs) =>
        TotalDurationMs == 0
            ? 0
            : Math.Round((TotalDurationMs / (double)(totalDurationMs)) * 100, 1);
}

public interface IAuthStateProvider
{
    public Task Login(string accessToken);
    public Task Logout();
}
