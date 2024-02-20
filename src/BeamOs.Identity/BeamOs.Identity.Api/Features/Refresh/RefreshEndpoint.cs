using System.Security.Claims;
using BeamOS.Common.Api;
using BeamOs.Identity.Api.Entities;
using BeamOs.Identity.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Identity.Api.Features.Refresh;

public class RefreshEndpoint(
    BeamOsIdentityDbContext dbContext,
    IHttpContextAccessor httpContextAccessor
) : BeamOsEndpoint<string, string>
{
    public override string Route => "/refresh";

    public override EndpointType EndpointType => EndpointType.Post;

    public override async Task<string> ExecuteAsync(string token, CancellationToken ct)
    {
        ClaimsPrincipal claimsPrincipal =
            httpContextAccessor.HttpContext?.User ?? throw new Exception(string.Empty);

        var emailClaim = claimsPrincipal.FindFirst(ClaimTypes.Email) ?? throw new Exception();
        BeamOsUser user =
            await dbContext
                .Users
                .SingleOrDefaultAsync(u => u.Email == emailClaim.Value, cancellationToken: ct)
            ?? throw new Exception();

        if (
            user.RefreshToken == RefreshToken.Default
            || !user.RefreshToken.PasswordMatches(token)
            || !user.RefreshToken.IsNotExpired(DateTime.UtcNow)
        )
        {
            throw new Exception();
        }

        string newTokenString = RefreshToken.GenerateUnhashedToken();
        //RefreshToken

        return null;
    }

    //private string CreateRefreshToken(RegisterUserRequest request)
    //{
    //    var x = new BeamOsUser();
    //    Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    //}
}
