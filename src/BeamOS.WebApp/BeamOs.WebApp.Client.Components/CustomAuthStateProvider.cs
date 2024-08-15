using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace BeamOs.WebApp.Client.Components;

public class CustomAuthStateProvider(Func<ILocalStorageService> localStorageServiceFactory)
    : AuthenticationStateProvider
{
    private static readonly AuthenticationState UnauthenticatedState = new(new ClaimsPrincipal());
    private AuthenticationState? authenticatedState;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return this.authenticatedState ?? await this.GetAuthStateFromLocalStorage();
    }

    public async Task Login(string jwt)
    {
        ILocalStorageService localStorageService = localStorageServiceFactory();
        await localStorageService.SetItemAsStringAsync(Constants.ACCESS_TOKEN_GUID, jwt);

        var principal = CreateClaimsPrincipalFromJwt(jwt);
        this.authenticatedState = new AuthenticationState(principal);
        this.NotifyAuthenticationStateChanged(Task.FromResult(this.authenticatedState));
    }

    public void LogOut()
    {
        this.authenticatedState = null;
        this.NotifyAuthenticationStateChanged(Task.FromResult(UnauthenticatedState));
    }

    private async Task<AuthenticationState> GetAuthStateFromLocalStorage()
    {
        ILocalStorageService localStorageService = localStorageServiceFactory();
        string? authToken = await localStorageService.GetItemAsStringAsync(
            Constants.ACCESS_TOKEN_GUID
        );

        if (string.IsNullOrWhiteSpace(authToken))
        {
            return UnauthenticatedState;
        }

        this.authenticatedState = new(CreateClaimsPrincipalFromJwt(authToken));

        return this.authenticatedState;
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
