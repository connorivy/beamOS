using System.Security.Cryptography;
using BeamOS.Common.Domain.Models;

namespace BeamOs.Identity.Api.Entities;

public class RefreshToken : BeamOSValueObject
{
    public RefreshToken(string token, DateTime creationDate, DateTime expiryDate)
    {
        this.tokenHash = BCrypt.Net.BCrypt.HashPassword(token);
        this.creationDate = creationDate;
        this.expiryDate = expiryDate;
    }

    private readonly string tokenHash;
    private readonly DateTime creationDate;
    private readonly DateTime expiryDate;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.tokenHash;
        yield return this.creationDate;
        yield return this.expiryDate;
    }

    public static string GenerateUnhashedToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public bool PasswordMatches(string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, this.tokenHash);
    }

    public bool IsNotExpired(DateTime currentTime)
    {
        return currentTime > this.creationDate && currentTime < this.expiryDate;
    }
}
