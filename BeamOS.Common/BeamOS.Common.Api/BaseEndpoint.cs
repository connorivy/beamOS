using BeamOS.Common.Api.Interfaces;
using FastEndpoints;

namespace BeamOS.Common.Api;

public abstract class BaseEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse>,
    IEndpoint<TRequest, TResponse>
    where TRequest : notnull
{
    public abstract override Task<TResponse> ExecuteAsync(TRequest req, CancellationToken ct);
}
