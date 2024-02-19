using BeamOS.Common.Api;
using BeamOs.Identity.Api.Entities;
using BeamOs.Identity.Api.Features.Common;
using BeamOs.Identity.Api.Features.Login;
using BeamOs.Identity.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Identity.Api.Features.Register;

public class RegisterEndpoint(
    BeamOsIdentityDbContext dbContext,
    LoginVerifiedUser loginVerifiedUser
) : BeamOsEndpoint<LoginRequest, bool>
{
    public override string Route => "/register";

    public override EndpointType EndpointType => EndpointType.Post;

    public override async Task<bool> ExecuteAsync(LoginRequest request, CancellationToken ct)
    {
        BeamOsUser? existingUser = await dbContext
            .Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken: ct);

        if (existingUser != null)
        {
            throw new Exception();
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        BeamOsUser newUser = new() { Email = request.Email, PasswordHash = passwordHash };
        var r = await dbContext.Users.AddAsync(newUser, ct);
        _ = await dbContext.SaveChangesAsync(ct);

        _ = await loginVerifiedUser.Login(newUser, "", ct);

        return true;
    }
}
