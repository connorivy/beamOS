using System.Security.Claims;
using BeamOS.Common.Api;
using BeamOs.Identity.Api.Entities;
using BeamOs.Identity.Api.Infrastructure;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BeamOs.Identity.Api.Features.LoginWithGoogle;

public class LoginWithGoogleEndpointRedirect(
    SignInManager<BeamOsUser> signInManager,
    UserManager<BeamOsUser> userManager,
    BeamOsIdentityDbContext dbContext,
    IHttpContextAccessor httpContextAccessor
) : BeamOsEndpoint<string, EmptyResponse>
{
    public const string RedirectUrlQueryParam = "RedirectUrl";
    public override string Route => "/login-with-google/authenticated";

    public override EndpointType EndpointType => EndpointType.Get;

    public override async Task<EmptyResponse> ExecuteAsync(
        [FromQuery] string redirectUrl,
        CancellationToken ct
    )
    {
        ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();

        var user = await userManager.GetUserAsync(info.Principal);
        string email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? throw new Exception();
        if (user == null)
        {
            user = new BeamOsUser() { Email = email, EmailConfirmed = true };
            await userManager.CreateAsync(user);
            _ = await dbContext.SaveChangesAsync(ct);
        }

        httpContextAccessor.HttpContext.Response.Redirect(redirectUrl);

        return new EmptyResponse();
    }
}
