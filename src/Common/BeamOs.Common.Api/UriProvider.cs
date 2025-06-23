namespace BeamOs.Common.Api;

public record UriProvider
{
    public Uri AiUri { get; init; } = new Uri("http://localhost:5223");
}
