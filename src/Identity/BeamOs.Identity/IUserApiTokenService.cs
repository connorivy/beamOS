using BeamOs.Common.Contracts;

namespace BeamOs.Identity;

public interface IUserApiTokenService
{
    public Task<Result<ApiTokenResponse>> CreateToken(CreateApiTokenRequest token);

    public Task<Result<List<ApiTokenResponse>>> GetTokens();

    public Task RevokeToken(string tokenName);
}

public class ExampleUserApiTokenService : IUserApiTokenService
{
    private List<ApiTokenResponse> apiTokenResponses =
    [
        new() { Name = "Mobile App Token", Scopes= ["models:read"], CreatedOn = DateTime.Now.AddDays(-5) },
        new() { Name = "CI/CD Pipeline Token", Scopes= ["models:read", "models:write"], CreatedOn = DateTime.Now.AddDays(-2) }
    ];

    public Task<Result<ApiTokenResponse>> CreateToken(CreateApiTokenRequest token)
    {
        ApiTokenResponse resp =
            new()
            {
                Name = token.Name,
                Scopes = token.Scopes,
                CreatedOn = DateTime.UtcNow,
                Value = Guid.NewGuid().ToString()
            };
        this.apiTokenResponses.Add(resp);

        Result<ApiTokenResponse> result = resp;
        return Task.FromResult(result);
    }

    public Task<Result<List<ApiTokenResponse>>> GetTokens()
    {
        Result<List<ApiTokenResponse>> result = this.apiTokenResponses;
        return Task.FromResult(result);
    }

    public Task RevokeToken(string tokenName)
    {
        this.apiTokenResponses = this.apiTokenResponses.Where(x => x.Name != tokenName).ToList();
        return Task.CompletedTask;
    }
}
