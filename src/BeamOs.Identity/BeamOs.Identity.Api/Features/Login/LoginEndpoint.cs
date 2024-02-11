using BeamOS.Common.Api;
using BeamOs.Identity.Api.Entities;
using BeamOs.Identity.Api.Infrastructure;
using BeamOs.Identity.Contracts.Users;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Identity.Api.Features.Login;

public class LoginEndpoint(IConfiguration configuration, BeamOsIdentityDbContext dbContext)
    : BeamOsEndpoint<LoginRequest, AuthenticationResponse>
{
    public override string Route => "/login";

    public override EndpointType EndpointType => EndpointType.Post;

    public override async Task<AuthenticationResponse> ExecuteAsync(
        LoginRequest request,
        CancellationToken ct
    )
    {
        BeamOsUser? existingUser =
            await dbContext
                .Users
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken: ct)
            ?? throw new Exception();

        if (
            existingUser is null
            || !BCrypt.Net.BCrypt.Verify(request.Password, existingUser.PasswordHash)
        )
        {
            //return BadRequest("Username and password don't match");
            return null;
        }

        string token = AccessToken.Create(
            request.Email,
            configuration["JwtSettings:Audiences"].Split(','),
            configuration["JwtSettings:Key"],
            configuration["JwtSettings:Issuer"]
        );

        string refreshToken = RefreshToken.GenerateUnhashedToken();
        existingUser.RefreshToken = new(refreshToken, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));
        _ = await dbContext.SaveChangesAsync(ct);

        return new AuthenticationResponse(token, refreshToken);
    }
}
