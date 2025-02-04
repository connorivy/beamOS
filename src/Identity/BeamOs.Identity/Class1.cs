using BeamOs.Common.Contracts;

namespace BeamOs.Identity;

public record CreateApiTokenRequest
{
    public required string Name { get; init; }
    public required List<string> Scopes { get; init; }
}

public record ApiTokenResponse
{
    public string Name { get; init; }
    public DateTime CreatedOn { get; init; }
    public List<string> Scopes { get; init; }
}

public interface IUserApiTokenService
{
    public Task<Result<ApiTokenResponse>> CreateToken(CreateApiTokenRequest token);

    public Task<Result<List<ApiTokenResponse>>> GetTokens();

    public Task RevokeToken(string tokenName);
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

public interface IUserApiUsageService
{
    public Task<ApiUsageResponse?> GetApiUsage();
}
