using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BeamOs.Identity.Api.Entities;

public class AccessTokenFactory(IConfiguration configuration)
{
    private readonly IEnumerable<string> audiences = configuration["JwtSettings:Audiences"].Split(
        ','
    );
    private readonly string keyString = configuration["JwtSettings:Key"];
    private readonly string issuer = configuration["JwtSettings:Issuer"];

    public string Create(List<Claim> claims)
    {
        foreach (var audience in this.audiences)
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, audience));
        }

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(this.keyString));

        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha512Signature);

        JwtSecurityToken token =
            new(
                issuer: this.issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string Create(BeamOsUser user)
    {
        List<Claim> claims =
        [
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.GivenName)
        ];
        return this.Create(claims);
    }
}
