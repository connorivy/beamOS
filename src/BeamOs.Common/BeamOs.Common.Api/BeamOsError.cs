namespace BeamOs.Common.Api;

public sealed record BeamOsError(string Code, string Description)
{
    public static readonly BeamOsError None = new(string.Empty, string.Empty);
}
