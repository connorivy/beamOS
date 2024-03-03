using FastEndpoints;

namespace BeamOS.Common.Api;

public class BeamOsFastEndpointOptions
{
    public Action<BaseEndpoint>? ConfigureAuthentication { get; }
}
