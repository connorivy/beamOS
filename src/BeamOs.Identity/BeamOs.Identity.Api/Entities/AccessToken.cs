using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BeamOs.Identity.Api.Entities;

public static class AccessToken
{
    public static string Create(
        string email,
        IEnumerable<string> audiences,
        string keyString,
        string issuer
    )
    {
        List<Claim> claims = [new Claim(ClaimTypes.Email, email)];

        foreach (var audience in audiences)
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, audience));
        }

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(keyString));

        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha512Signature);

        JwtSecurityToken token =
            new(
                issuer: issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
