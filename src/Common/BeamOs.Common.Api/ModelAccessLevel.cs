namespace BeamOs.Common.Api;

public static class UserAuthorizationLevel
{
    public const string None = nameof(None);
    public const string Authenticated = nameof(Authenticated);
    public const string Reviewer = nameof(Reviewer);
    public const string Contributor = nameof(Contributor);
    public const string Owner = nameof(Owner);
}

public record UriProvider
{
    public Uri AiUri { get; init; } = new Uri("http://localhost:5223");
}
