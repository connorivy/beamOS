using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BeamOS.Common.Api;
using BeamOs.Identity.Api.Users.Common;
using BeamOs.Identity.Contracts.Users;
using Microsoft.IdentityModel.Tokens;

namespace BeamOs.Identity.Api.Users.Endpoints;

public class Login(IConfiguration configuration)
    : BeamOsEndpoint<RegisterUserRequest, AuthenticationResponse>
{
    public override string Route => "/login";

    public override EndpointType EndpointType => EndpointType.Post;

    public override Task<AuthenticationResponse> ExecuteAsync(
        RegisterUserRequest request,
        CancellationToken ct
    )
    {
        string token = this.CreateToken(request);

        return Task.FromResult(new AuthenticationResponse(token, null));
    }

    private string CreateToken(RegisterUserRequest request)
    {
        List<Claim> claims = [new Claim(ClaimTypes.Email, request.Email)];

        foreach (string audience in configuration["JwtSettings:Audiences"].Split(','))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, audience));
        }

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]));

        SigningCredentials cred = new(key, SecurityAlgorithms.HmacSha512Signature);

        JwtSecurityToken token =
            new(
                issuer: configuration["JwtSettings:Issuer"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: cred
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
