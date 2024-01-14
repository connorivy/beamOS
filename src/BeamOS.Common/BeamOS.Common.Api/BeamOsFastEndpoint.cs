using BeamOS.Common.Api.Interfaces;
using FastEndpoints;

namespace BeamOS.Common.Api;

public abstract class BeamOsFastEndpoint<TRequest, TResponse>
    : Endpoint<TRequest, TResponse>,
        IBeamOsEndpoint<TRequest, TResponse>
    where TRequest : notnull
{
    public abstract override void Configure();

    public abstract override Task<TResponse> ExecuteAsync(TRequest req, CancellationToken ct);
}
