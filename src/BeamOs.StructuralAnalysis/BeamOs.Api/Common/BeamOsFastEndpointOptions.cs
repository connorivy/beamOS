using FastEndpoints;

namespace BeamOS.Api;

public class BeamOsFastEndpointOptions
{
    public Action<BaseEndpoint>? DefaultAuthenticationConfiguration { get; }
}
