using BeamOS.Common.Api;
using BeamOs.Identity.Api.Entities;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Primitives;

namespace BeamOs.Identity.Api.Features.LoginWithGoogle;

public class LoginWithGoogleEndpoint(
    SignInManager<BeamOsUser> signInManager,
    IHttpContextAccessor httpContextAccessor,
    LoginWithGoogleEndpointRedirect loginWithGoogleEndpointRedirect
) : BeamOsFastEndpoint<ExternalLoginRequest, IResult>
{
    //public override string Route => "/login-with-google";

    //public override EndpointType EndpointType => EndpointType.Post;

    public override void Configure()
    {
        this.Get("/login-with-google");
        this.AllowAnonymous();
    }

    public override Task<IResult> ExecuteAsync(ExternalLoginRequest req, CancellationToken ct)
    {
        IEnumerable<KeyValuePair<string, StringValues>> query =
        [
            new(LoginWithGoogleEndpointRedirect.RedirectUrlQueryParam, req.ReturnUrl)
        ];

        var provider = GoogleDefaults.AuthenticationScheme;
        var redirectUrl = UriHelper.BuildRelative(
            httpContextAccessor.HttpContext.Request.PathBase,
            loginWithGoogleEndpointRedirect.Route,
            QueryString.Create(query)
        );

        var properties = signInManager.ConfigureExternalAuthenticationProperties(
            provider,
            redirectUrl
        );

        var result = TypedResults.Challenge(properties, [provider]);

        return Task.FromResult(result as IResult);
    }
}
