using System.Security.Claims;
using BeamOS.Common.Api;
using BeamOs.Identity.Api.Entities;
using BeamOs.Identity.Api.Features.Common;
using BeamOs.Identity.Api.Infrastructure;
using FastEndpoints;
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
) : BeamOsEndpoint<string, string>
{
    public const string RedirectUrlQueryParam = "RedirectUrl";
    public const string LocalStorageUrlQueryParam = "LocalStorageUrl";
    public override string Route => "/login-with-google/authenticated";

    public override EndpointType EndpointType => EndpointType.Get;

    public override async Task<string> ExecuteAsync(
        [FromQuery] string redirectUrl,
        CancellationToken ct
    )
    {
        ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();

        string email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? throw new Exception();
        string? givenName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
        string? surname = info.Principal.FindFirstValue(ClaimTypes.Surname);

        BeamOsUser? user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new BeamOsUser()
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                GivenName = givenName,
                Surname = surname
            };
            _ = await userManager.CreateAsync(user);
            _ = await dbContext.SaveChangesAsync(ct);
        }

        var authResponse = await authenticationResponseFactory.Create(user, ct);

        //using HttpRequestMessage authResponseMessage = CreateAuthResponseMessage(
        //    authResponse,
        //    "https://localhost:7111/auth-state-changed"
        //);
        //authResponseMessage.Headers.Add("beamOS-access-token", authResponse.AccessToken);
        //HttpClient client = new();
        //var x = await client.SendAsync(authResponseMessage, ct);

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

    private HttpRequestMessage CreateAuthResponseMessage(
        AuthenticationResponse authenticationResponse,
        string url
    )
    {
        var request_ = new System.Net.Http.HttpRequestMessage();

        var json_ = System.Text.Json.JsonSerializer.Serialize(authenticationResponse);
        var content_ = new System.Net.Http.StringContent(json_);
        content_.Headers.ContentType = System
            .Net
            .Http
            .Headers
            .MediaTypeHeaderValue
            .Parse("application/json");
        request_.Content = content_;
        request_.Method = new System.Net.Http.HttpMethod("GET");
        request_
            .Headers
            .Accept
            .Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

        request_.RequestUri = new Uri(url, UriKind.Absolute);

        return request_;
    }
}
