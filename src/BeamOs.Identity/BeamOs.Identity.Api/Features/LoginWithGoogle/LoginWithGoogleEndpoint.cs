using BeamOS.Common.Api;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Primitives;

namespace BeamOs.Identity.Api.Features.LoginWithGoogle;

public class LoginWithGoogleEndpoint : BeamOsEndpoint<string, IResult>
{
    public override string Route => "/login-with-google";

    public override EndpointType EndpointType => EndpointType.Post;

    public override async Task<IResult> ExecuteAsync(string request, CancellationToken ct)
    {
        //var x = "";
        //var context = httpContextAccessor.HttpContext;
        //if (!await antiforgery.IsRequestValidAsync(context))
        //{
        //    return Results.Redirect(RootPath);
        //}
        await Task.CompletedTask;

        return Results.Challenge(
            new() { RedirectUri = $"https://localhost:7194/{this.Route}" },
            [GoogleDefaults.AuthenticationScheme]
        );
    }
}
