using FastEndpoints;

namespace BeamOs.Api.Common;

public class BeamOsFastEndpointOptions
{
    public Action<BaseEndpoint>? DefaultAuthenticationConfiguration { get; }
}
