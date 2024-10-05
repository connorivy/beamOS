using System.Security.Claims;
using System.Text.Json;
using BeamOs.Common.Identity;
using Microsoft.AspNetCore.Components.Authorization;

namespace BeamOS.WebApp.Components.Providers;

public class CustomAuthStateProvider
    : AuthenticationStateProvider,
        IHostEnvironmentAuthenticationStateProvider,
        IAuthStateProvider
{
    private static readonly AuthenticationState UnauthenticatedState = new(new ClaimsPrincipal());

    private Task<AuthenticationState>? authenticationStateTask;

    public CustomAuthStateProvider()
    {
        // log in the user by default
        this.Login(
            "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJ1c2VyQGVtYWlsLmNvbSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJVc2VyIiwiYXVkIjpbImh0dHBzOi8vbG9jYWxob3N0OjcxOTMiLCJodHRwczovL2xvY2FsaG9zdDo3MTk0Il0sImV4cCI6NDg3MDA5MDM2MCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzE5NCJ9.CTW9SNJl4kSvAlJGZ7dDpFsCc-6hVeOu6OhJFllzGwkE2FZwBd34i7q8nIaKQDQf3T8-O-GqyF7Jbey2ULDPOA"
        );
    }

    /// <inheritdoc />
    public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
        this.authenticationStateTask ?? Task.FromResult(UnauthenticatedState);

    /// <inheritdoc />
    public void SetAuthenticationState(Task<AuthenticationState> authenticationStateTask)
    {
        // don't override the logged in user unless access token is null
        if (
            this.accessToken != null
            && !authenticationStateTask.Result.User.Identity.IsAuthenticated
        )
        {
            return;
        }
        this.SetAuthenticationStatePrivate(authenticationStateTask);
    }

    public void SetAuthenticationStatePrivate(Task<AuthenticationState> authenticationStateTask)
    {
        this.authenticationStateTask =
            authenticationStateTask
            ?? throw new ArgumentNullException(nameof(authenticationStateTask));

        this.NotifyAuthenticationStateChanged(this.authenticationStateTask);
    }

    private string? accessToken;

    public string? GetAccessToken() => this.accessToken;

    public Task Login(string accessToken)
    {
        var principal = CreateClaimsPrincipalFromJwt(accessToken);
        this.SetAuthenticationStatePrivate(Task.FromResult(new AuthenticationState(principal)));
        this.accessToken = accessToken;
        return Task.CompletedTask;
    }

    public Task Logout()
    {
        this.accessToken = null;
        this.SetAuthenticationStatePrivate(Task.FromResult(UnauthenticatedState));
        return Task.CompletedTask;
    }

    private static ClaimsPrincipal CreateClaimsPrincipalFromJwt(string jwt)
    {
        return new(new ClaimsIdentity(ParseClaimsFromJwt(jwt), "beamOsJwt"));
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2:
                base64 += "==";
                break;
            case 3:
                base64 += "=";
                break;
            default:
                break;
        }
        return Convert.FromBase64String(base64);
    }
}
