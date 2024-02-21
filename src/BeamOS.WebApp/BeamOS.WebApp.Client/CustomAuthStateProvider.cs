using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace BeamOS.WebApp.Client;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string? token = null;

        var identity = new ClaimsIdentity();
        //http.DefaultRequestHeaders.Authorization = null;

        if (!string.IsNullOrEmpty(token))
        {
            identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            //http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            //    "Bearer",
            //    token.Replace("\"", "")
            //);
        }

        var user = new ClaimsPrincipal(identity);
        var state = new AuthenticationState(user);

        var result = Task.FromResult(state);
        this.NotifyAuthenticationStateChanged(result);

        return result;
    }

    public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
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
