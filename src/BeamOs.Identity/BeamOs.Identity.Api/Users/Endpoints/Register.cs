using BeamOS.Common.Api;
using BeamOs.Identity.Api.Users.Common;
using BeamOs.Identity.Infrastructure;

namespace BeamOs.Identity.Api.Users.Endpoints;

public class Register : BeamOsEndpoint<RegisterUserRequest, bool>
{
    public override string Route => "/register";

    public override EndpointType EndpointType => EndpointType.Post;

    public override async Task<bool> ExecuteAsync(RegisterUserRequest request, CancellationToken ct)
    {
        await Task.CompletedTask;

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        BeamOsUser user = new BeamOsUser() { Email = request.Email, PasswordHash = passwordHash, };

        return true;
    }
}
