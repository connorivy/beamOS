using BeamOs.StructuralAnalysis.Contracts.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.StructuralAnalysis.Api;

public interface IBaseEndpoint
{
    public static abstract string Route { get; }
    public static abstract string EndpointName { get; }
    public static abstract Http EndpointType { get; }
}

public interface IBaseEndpoint<TRequest, TResponse> : IBaseEndpoint
{
    public static abstract Func<HttpRequest, Task<TRequest>> RequestObjectBinder { get; }

    public Task<Result<TResponse>> ExecuteRequestAsync(
        TRequest req,
        CancellationToken ct = default
    );

    public async Task<IActionResult> RunExecuteAsync<TEndPoint>(HttpRequest req)
        where TEndPoint : IBaseEndpoint<TRequest, TResponse>
    {
        try
        {
            TRequest typedRequest = await TEndPoint.RequestObjectBinder(req);
            var result = await this.ExecuteRequestAsync(typedRequest);

            return new OkObjectResult(result);
        }
        catch (Exception ex)
        {
            return new OkObjectResult(
                new { Message = ex.Message, InnerException = ex.InnerException?.Message }
            );
        }
    }
    public Task<IActionResult> Run(HttpRequest req);
}

public static class EndpointToMinimalApi
{
    public static void Map<TEndpoint, TRequest, TResponse>(IEndpointRouteBuilder app)
        where TEndpoint : IBaseEndpoint<TRequest, TResponse>
    {
        IEndpointConventionBuilder endpointBuilder;

        if (TEndpoint.EndpointType is Http.POST)
        {
            endpointBuilder = app.MapPost(
                TEndpoint.Route,
                async ([AsParameters] TRequest req, IServiceProvider serviceProvider) =>
                    await serviceProvider.GetRequiredService<TEndpoint>().ExecuteRequestAsync(req)
            );
        }
        else if (TEndpoint.EndpointType is Http.GET)
        {
            endpointBuilder = app.MapGet(
                TEndpoint.Route,
                async ([AsParameters] TRequest req, IServiceProvider serviceProvider) =>
                    await serviceProvider.GetRequiredService<TEndpoint>().ExecuteRequestAsync(req)
            );
        }
        else if (TEndpoint.EndpointType is Http.PATCH)
        {
            endpointBuilder = app.MapPatch(
                TEndpoint.Route,
                async ([AsParameters] TRequest req, IServiceProvider serviceProvider) =>
                    await serviceProvider.GetRequiredService<TEndpoint>().ExecuteRequestAsync(req)
            );
        }
        else if (TEndpoint.EndpointType is Http.PUT)
        {
            endpointBuilder = app.MapPut(
                TEndpoint.Route,
                async ([AsParameters] TRequest req, IServiceProvider serviceProvider) =>
                    await serviceProvider.GetRequiredService<TEndpoint>().ExecuteRequestAsync(req)
            );
        }
        else
        {
            throw new NotImplementedException();
        }

        endpointBuilder.WithName(TEndpoint.EndpointName);
    }
}

public interface IResultHandler
{
    public static IActionResult HandleActionResult<TResponse>(Result<TResponse> response)
    {
        return response.Match<IActionResult>(
            r => new OkObjectResult(r),
            ex => new ObjectResult(new { message = "Internal Server Error", details = ex.Message })
            {
                StatusCode = StatusCodes.Status500InternalServerError,
            }
        );
    }

    //public static IHttpResult HandleHttpResult<TResponse>(Result<TResponse> response)
    //{
    //    return response.Match<IActionResult>(
    //        r => new HttpResults.Ok(r),
    //        ex => new ObjectResult(new { message = "Internal Server Error", details = ex.Message })
    //        {
    //            StatusCode = StatusCodes.Status500InternalServerError,
    //        }
    //    );
    //}
}
