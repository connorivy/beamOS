namespace BeamOS.Common.Api.Interfaces;

public interface IBeamOsEndpointBase { }

public interface IBeamOsEndpoint<TRequest, TResponse> : IBeamOsEndpointBase
{
    public Task<TResponse> ExecuteAsync(TRequest request, CancellationToken ct);
}

public interface IBeamOsEndpoint<TRequest, TParam1, TResponse> : IBeamOsEndpointBase
{
    public Task<TResponse> ExecuteAsync(TRequest request, TParam1 param1, CancellationToken ct);
}

public interface IBeamOsEndpoint<TRequest, TParam1, TParam2, TResponse> : IBeamOsEndpointBase
{
    public Task<TResponse> ExecuteAsync(
        TRequest request,
        TParam1 param1,
        TParam2 param2,
        CancellationToken ct
    );
}

public interface IBeamOsEndpoint<TRequest, TParam1, TParam2, TParam3, TResponse>
    : IBeamOsEndpointBase
{
    public Task<TResponse> ExecuteAsync(
        TRequest request,
        TParam1 param1,
        TParam2 param2,
        TParam3 param3,
        CancellationToken ct
    );
}
