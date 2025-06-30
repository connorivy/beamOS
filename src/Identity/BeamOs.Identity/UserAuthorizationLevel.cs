namespace BeamOs.Common.Api;

public static class UserAuthorizationLevel
{
    public const string None = nameof(None);
    public const string Authenticated = nameof(Authenticated);
    public const string Reviewer = nameof(Reviewer);
    public const string Proposer = nameof(Proposer);
    public const string Contributor = nameof(Contributor);
    public const string Owner = nameof(Owner);
}
