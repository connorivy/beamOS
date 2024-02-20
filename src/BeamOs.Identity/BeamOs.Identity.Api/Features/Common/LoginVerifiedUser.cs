using BeamOs.Identity.Api.Entities;

namespace BeamOs.Identity.Api.Features.Common;

public class LoginVerifiedUser(
    AuthenticationResponseFactory authenticationResponseFactory,
    IHttpContextAccessor httpContextAccessor
)
{
    public async Task<string> Login(BeamOsUser user, string redirectUrl, CancellationToken ct)
    {
        var authResponse = await authenticationResponseFactory.Create(user, ct);

        CookieOptions authOptions = new() { HttpOnly = true, };
        httpContextAccessor
            .HttpContext
            .Response
            .Cookies
            .Append("Authorization", authResponse.AccessToken, authOptions);
        httpContextAccessor
            .HttpContext
            .Response
            .Cookies
            .Append("Refresh", authResponse.RefreshToken, authOptions);

        httpContextAccessor.HttpContext.Response.Redirect(redirectUrl);

        return string.Empty;
    }
}
