using FastEndpoints;

namespace BeamOs.Api.Common.Interfaces;

public interface IBeamOsEndpointBase
{
    public abstract string Route { get; }

    public abstract Http EndpointType { get; }
}

public interface IBeamOsEndpoint<TRequest, TResponse> : IBeamOsEndpointBase
{
    public Task<TResponse> ExecuteRequestAsync(TRequest request, CancellationToken ct);
}
