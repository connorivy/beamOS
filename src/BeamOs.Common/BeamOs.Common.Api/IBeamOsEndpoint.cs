namespace BeamOs.Common.Api;

public interface IBeamOsEndpointBase
{
    public string Route { get; }

    public object ExecuteRequestAsync(object request, CancellationToken ct);
}

public interface IBeamOsEndpoint<TRequest, TResponse> : IBeamOsEndpointBase
{
    public Task<TResponse> ExecuteRequestAsync(TRequest request, CancellationToken ct);
}
