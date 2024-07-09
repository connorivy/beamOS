namespace BeamOs.Common.Api;

public sealed record BeamOsError
{
    public required string Code { get; init; }
    public required string Description { get; init; }

    public static readonly BeamOsError None =
        new() { Code = string.Empty, Description = string.Empty };

    public static BeamOsError ServiceNotFound(string serviceName)
    {
        return new()
        {
            Code = "ServiceNotFound",
            Description = $"Service of type {serviceName} was not found"
        };
    }
}
