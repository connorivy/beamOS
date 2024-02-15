using BeamOs.Identity.Api.Infrastructure;
using BeamOs.Identity.Contracts.Users;

namespace BeamOs.Identity.Api.Entities;

public class AuthenticationResponseFactory(
    BeamOsIdentityDbContext dbContext,
    AccessTokenFactory accessTokenFactory
)
{
    public async Task<AuthenticationResponse> Create(
        BeamOsUser user,
        CancellationToken ct = default
    )
    {
        string token = accessTokenFactory.Create(user);

        string refreshToken = RefreshToken.GenerateUnhashedToken();
        user.RefreshToken = new(refreshToken, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));
        _ = await dbContext.SaveChangesAsync(ct);

        return new AuthenticationResponse(token, refreshToken);
    }
}
