using BeamOS.Common.Api;
using BeamOs.Identity.Api.Entities;
using BeamOs.Identity.Api.Features.Common;
using BeamOs.Identity.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Identity.Api.Features.Login;

public class LoginEndpoint(
    AuthenticationResponseFactory authenticationResponseFactory,
    BeamOsIdentityDbContext dbContext,
    LoginVerifiedUser loginVerifiedUser,
    AccessTokenFactory accessTokenFactory
) : BeamOsEndpoint<LoginRequest, AuthenticationResponse>
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

        var x = accessTokenFactory.Create(existingUser);

        if (
            existingUser is null
            || !BCrypt.Net.BCrypt.Verify(request.Password, existingUser.PasswordHash)
        )
        {
            //return BadRequest("Username and password don't match");
            return null;
        }

        _ = await loginVerifiedUser.Login(existingUser, "", ct);

        return await authenticationResponseFactory.Create(existingUser, ct);
    }
}
