using System.Security.Claims;
using System.Text.Json;
using BeamOs.Common.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;

namespace BeamOS.WebApp.Components.Providers;

public class CustomAuthStateProvdier
    : AuthenticationStateProvider,
        IHostEnvironmentAuthenticationStateProvider,
        IAuthStateProvider
{
    private static readonly AuthenticationState UnauthenticatedState = new(new ClaimsPrincipal());

    private Task<AuthenticationState>? authenticationStateTask;

    /// <inheritdoc />
    public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
        authenticationStateTask
        ?? throw new InvalidOperationException(
            $"Do not call {nameof(GetAuthenticationStateAsync)} outside of the DI scope for a Razor component. Typically, this means you can call it only within a Razor component or inside another DI service that is resolved for a Razor component."
        );

    /// <inheritdoc />
    public void SetAuthenticationState(Task<AuthenticationState> authenticationStateTask)
    {
        // this block logs in the user to a default profile
        if (this.authenticationStateTask is null)
        {
            this.Login(
                "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJ1c2VyQGVtYWlsLmNvbSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJVc2VyIiwiYXVkIjpbImh0dHBzOi8vbG9jYWxob3N0OjcxOTMiLCJodHRwczovL2xvY2FsaG9zdDo3MTk0Il0sImV4cCI6NDg3MDA5MDM2MCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzE5NCJ9.CTW9SNJl4kSvAlJGZ7dDpFsCc-6hVeOu6OhJFllzGwkE2FZwBd34i7q8nIaKQDQf3T8-O-GqyF7Jbey2ULDPOA"
            );
        }
        else
        {
            this.SetAuthenticationStatePrivate(authenticationStateTask);
        }
    }

    public void SetAuthenticationStatePrivate(Task<AuthenticationState> authenticationStateTask)
    {
        this.authenticationStateTask =
            authenticationStateTask
            ?? throw new ArgumentNullException(nameof(authenticationStateTask));

        this.NotifyAuthenticationStateChanged(this.authenticationStateTask);
    }

    public Task Login(string accessToken)
    {
        var principal = CreateClaimsPrincipalFromJwt(accessToken);
        this.SetAuthenticationStatePrivate(Task.FromResult(new AuthenticationState(principal)));
        return Task.CompletedTask;
    }

    public Task Logout()
    {
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
