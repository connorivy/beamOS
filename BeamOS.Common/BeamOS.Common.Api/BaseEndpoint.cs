using BeamOS.Common.Api.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Routing;

namespace BeamOS.Common.Api;

public abstract class BaseEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse>,
    IEndpoint<TRequest, TResponse>
    where TRequest : notnull
{
    public void Map(IEndpointRouteBuilder app)
    {
        //app.MapGet("/items/{id:string}", GetItemById);
    }
    public abstract override Task<TResponse> ExecuteAsync(TRequest req, CancellationToken ct);
}

