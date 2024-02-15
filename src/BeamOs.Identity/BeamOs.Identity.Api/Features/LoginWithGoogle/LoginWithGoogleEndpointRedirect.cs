using System;
using System.Security.Claims;
using BeamOS.Common.Api;
using BeamOs.Identity.Api.Entities;
using BeamOs.Identity.Api.Infrastructure;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Identity.Api.Features.LoginWithGoogle;

public class LoginWithGoogleEndpointRedirect(
    SignInManager<BeamOsUser> signInManager,
    UserManager<BeamOsUser> userManager,
    BeamOsIdentityDbContext dbContext,
    IHttpContextAccessor httpContextAccessor,
    AuthenticationResponseFactory authenticationResponseFactory
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

        string email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? throw new Exception();
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            user = new BeamOsUser() { Email = email, EmailConfirmed = true };
            _ = await userManager.CreateAsync(user);
            _ = await dbContext.SaveChangesAsync(ct);
        }

        var cookie = httpContextAccessor.HttpContext.Request.Cookies[
            IdentityConstants.ExternalScheme
        ];

        var authResponse = await authenticationResponseFactory.Create(user);

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

        return new EmptyResponse();
    }
}
