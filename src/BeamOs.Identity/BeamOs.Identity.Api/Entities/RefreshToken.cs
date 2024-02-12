using System.Security.Cryptography;
using BeamOS.Common.Domain.Models;

namespace BeamOs.Identity.Api.Entities;

public class RefreshToken : BeamOSValueObject
{
    public RefreshToken(string token, DateTime creationDate, DateTime expiryDate)
    {
        this.TokenHash = BCrypt.Net.BCrypt.HashPassword(token);
        this.CreationDate = creationDate;
        this.ExpiryDate = expiryDate;
    }

    public string TokenHash { get; private set; }
    public DateTime CreationDate { get; private set; }
    public DateTime ExpiryDate { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.TokenHash;
        yield return this.CreationDate;
        yield return this.ExpiryDate;
    }

    public static string GenerateUnhashedToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public bool PasswordMatches(string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, this.TokenHash);
    }

    public bool IsNotExpired(DateTime currentTime)
    {
        return currentTime > this.CreationDate && currentTime < this.ExpiryDate;
    }

    // Currently, ef core doesn't allow database object to contain null complex properties
    // AKA, BeamOsUser.RefreshToken cannot be null. Therefore, create your own "default" as
    // a hack for now
    public static RefreshToken Default { get; } = new();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private RefreshToken() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
