using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BeamOs.Common.Identity;
using Microsoft.IdentityModel.Tokens;

namespace BeamOs.WebApp.Client.Components.Repositories;

public class AccountService : IAccountService
{
    private readonly Dictionary<string, User> hardCodedAccounts =
        new()
        {
            { "user@email.com", new("User", "user@email.com", "Password1!", "User") },
            { "admin@email.com", new("Admin", "admin@email.com", "Password1!", "Admin") },
        };

    public Task<AuthenticationResponse> LoginWithCredentials(string email, string password)
    {
        if (!this.hardCodedAccounts.TryGetValue(email, out var user))
        {
            throw new Exception("user not found");
        }

        if (password != user.Password)
        {
            throw new Exception("incorrect password");
        }

        return Task.FromResult(new AuthenticationResponse(CreateJWT(user), "todo"));
    }

    public Task<AuthenticationResponse> LoginWithProvider(string authProvider) =>
        throw new NotImplementedException();

    public Task Logout() => throw new NotImplementedException();

    private static string CreateJWT(User user)
    {
        var secretkey = new SymmetricSecurityKey(
            System
                .Text
                .Encoding
                .UTF8
                .GetBytes(
                    "superSecretHashKey.ThisIsJustADummyValue.Don'tHard-CodeYourRealKeyLikeThis"
                )
        );
        var credentials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserName), // NOTE: this will be the "User.Identity.Name" value
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
        };

        var token = new JwtSecurityToken(
            issuer: "fake",
            audience: "fake",
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record User(string UserName, string Email, string Password, string Role);
