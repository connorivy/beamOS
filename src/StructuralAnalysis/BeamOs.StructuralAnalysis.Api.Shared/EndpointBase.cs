using BeamOs.Common.Api;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.StructuralAnalysis.Api;

public abstract partial class BeamOsBaseEndpoint<TRequest, TResponse>
{
    public abstract string Route { get; }
    public abstract string EndpointName { get; }
    public abstract Http EndpointType { get; }
    public abstract UserAuthorizationLevel RequiredAccessLevel { get; }
    public abstract Task<Result<TResponse>> ExecuteRequestAsync(
        TRequest req,
        CancellationToken ct = default
    );

    public void Map(IEndpointRouteBuilder app)
    {
        IEndpointConventionBuilder endpointBuilder;

        if (this.EndpointType is Http.Post)
        {
            endpointBuilder = app.MapPost(
                this.Route,
                async ([AsParameters] TRequest req, IServiceProvider serviceProvider) =>
                    await this.ExecuteRequestAsync(req)
            );
        }
        else if (this.EndpointType is Http.Get)
        {
            endpointBuilder = app.MapGet(
                this.Route,
                async ([AsParameters] TRequest req, IServiceProvider serviceProvider) =>
                    await this.ExecuteRequestAsync(req)
            );
        }
        else if (this.EndpointType is Http.Patch)
        {
            endpointBuilder = app.MapPatch(
                this.Route,
                async ([AsParameters] TRequest req, IServiceProvider serviceProvider) =>
                    await this.ExecuteRequestAsync(req)
            );
        }
        else if (this.EndpointType is Http.Put)
        {
            endpointBuilder = app.MapPut(
                this.Route,
                async ([AsParameters] TRequest req, IServiceProvider serviceProvider) =>
                    await this.ExecuteRequestAsync(req)
            );
        }
        else
        {
            throw new NotImplementedException();
        }

        endpointBuilder.WithName(this.EndpointName);
    }
}

public abstract partial class BeamOsModelResourceBaseEndpoint<TCommand, TBody, TResponse>
    : BeamOsBaseEndpoint<TCommand, TResponse>
    where TCommand : IModelResourceRequest<TBody>, new() { }

public interface IBaseEndpoint
{
    public static abstract string Route { get; }
    public static abstract string EndpointName { get; }
    public static abstract Http EndpointType { get; }
}

public interface IBaseEndpoint<TRequest, TResponse> : IBaseEndpoint
{
    //public static abstract Func<HttpRequest, Task<TRequest>> RequestObjectBinder { get; }

    public static abstract UserAuthorizationLevel RequiredAccessLevel { get; }

    public Task<Result<TResponse>> ExecuteRequestAsync(
        TRequest req,
        CancellationToken ct = default
    );

    //public async Task<IActionResult> RunExecuteAsync<TEndPoint>(HttpRequest req)
    //    where TEndPoint : IBaseEndpoint<TRequest, TResponse>
    //{
    //    try
    //    {
    //        TRequest typedRequest = await TEndPoint.RequestObjectBinder(req);
    //        var result = await this.ExecuteRequestAsync(typedRequest);

    //        return new OkObjectResult(result);
    //    }
    //    catch (Exception ex)
    //    {
    //        return new OkObjectResult(
    //            new { Message = ex.Message, InnerException = ex.InnerException?.Message }
    //        );
    //    }
    //}
    //public Task<IActionResult> Run(HttpRequest req);
}

public static class EndpointToMinimalApi
{
    public static void Map<TEndpoint, TRequest, TResponse>(IEndpointRouteBuilder app)
        where TEndpoint : IBaseEndpoint<TRequest, TResponse>
    {
        IEndpointConventionBuilder endpointBuilder;

        if (TEndpoint.EndpointType is Http.Post)
        {
            endpointBuilder = app.MapPost(
                TEndpoint.Route,
                async ([AsParameters] TRequest req, IServiceProvider serviceProvider) =>
                    await serviceProvider.GetRequiredService<TEndpoint>().ExecuteRequestAsync(req)
            );
        }
        else if (TEndpoint.EndpointType is Http.Get)
        {
            endpointBuilder = app.MapGet(
                TEndpoint.Route,
                async ([AsParameters] TRequest req, IServiceProvider serviceProvider) =>
                    await serviceProvider.GetRequiredService<TEndpoint>().ExecuteRequestAsync(req)
            );
        }
        else if (TEndpoint.EndpointType is Http.Patch)
        {
            endpointBuilder = app.MapPatch(
                TEndpoint.Route,
                async ([AsParameters] TRequest req, IServiceProvider serviceProvider) =>
                    await serviceProvider.GetRequiredService<TEndpoint>().ExecuteRequestAsync(req)
            );
        }
        else if (TEndpoint.EndpointType is Http.Put)
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

    public static void Map2<TEndpoint, TRequest, TResponse>(IEndpointRouteBuilder app)
        where TEndpoint : BeamOsBaseEndpoint<TRequest, TResponse>
    {
        IEndpointConventionBuilder endpointBuilder;

        var endpointInstance = app.ServiceProvider.GetRequiredService<TEndpoint>();
        if (endpointInstance.EndpointType is Http.Post)
        {
            endpointBuilder = app.MapPost(
                endpointInstance.Route,
                async ([AsParameters] TRequest req, IServiceProvider serviceProvider) =>
                    await serviceProvider.GetRequiredService<TEndpoint>().ExecuteRequestAsync(req)
            );
        }
        else if (endpointInstance.EndpointType is Http.Get)
        {
            endpointBuilder = app.MapGet(
                endpointInstance.Route,
                async ([AsParameters] TRequest req, IServiceProvider serviceProvider) =>
                    await serviceProvider.GetRequiredService<TEndpoint>().ExecuteRequestAsync(req)
            );
        }
        else if (endpointInstance.EndpointType is Http.Patch)
        {
            endpointBuilder = app.MapPatch(
                endpointInstance.Route,
                async ([AsParameters] TRequest req, IServiceProvider serviceProvider) =>
                    await serviceProvider.GetRequiredService<TEndpoint>().ExecuteRequestAsync(req)
            );
        }
        else if (endpointInstance.EndpointType is Http.Put)
        {
            endpointBuilder = app.MapPut(
                endpointInstance.Route,
                async ([AsParameters] TRequest req, IServiceProvider serviceProvider) =>
                    await serviceProvider.GetRequiredService<TEndpoint>().ExecuteRequestAsync(req)
            );
        }
        else
        {
            throw new NotImplementedException();
        }

        endpointBuilder.WithName(endpointInstance.EndpointName);
    }
}

public interface IResultHandler
{
    public static IActionResult HandleActionResult<TResponse>(Result<TResponse> response)
    {
        return response.Match<IActionResult>(
            r => new OkObjectResult(r),
            ex =>
                new ObjectResult(new { message = "Internal Server Error", details = ex.Message })
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
