using BeamOs.Common.Application;
using BeamOs.Common.Contracts;

namespace BeamOs.Common.Api;

public abstract partial class BeamOsBaseEndpoint<TRequest, TResponse>
{
    //public abstract string Route { get; }
    //public abstract string EndpointName { get; }
    //public abstract Http EndpointType { get; }
    //public abstract UserAuthorizationLevel RequiredAccessLevel { get; }
    public abstract Task<Result<TResponse>> ExecuteRequestAsync(
        TRequest req,
        CancellationToken ct = default
    );

    //public void Map(IEndpointRouteBuilder app)
    //{
    //    IEndpointConventionBuilder endpointBuilder;

    //    if (this.EndpointType is Http.POST)
    //    {
    //        endpointBuilder = app.MapPost(
    //            this.Route,
    //            async ([AsParameters] TRequest req, IServiceProvider serviceProvider) =>
    //                await this.ExecuteRequestAsync(req)
    //        );
    //    }
    //    else if (this.EndpointType is Http.GET)
    //    {
    //        endpointBuilder = app.MapGet(
    //            this.Route,
    //            async ([AsParameters] TRequest req, IServiceProvider serviceProvider) =>
    //                await this.ExecuteRequestAsync(req)
    //        );
    //    }
    //    else if (this.EndpointType is Http.PATCH)
    //    {
    //        endpointBuilder = app.MapPatch(
    //            this.Route,
    //            async ([AsParameters] TRequest req, IServiceProvider serviceProvider) =>
    //                await this.ExecuteRequestAsync(req)
    //        );
    //    }
    //    else if (this.EndpointType is Http.PUT)
    //    {
    //        endpointBuilder = app.MapPut(
    //            this.Route,
    //            async ([AsParameters] TRequest req, IServiceProvider serviceProvider) =>
    //                await this.ExecuteRequestAsync(req)
    //        );
    //    }
    //    else
    //    {
    //        throw new NotImplementedException();
    //    }

    //    endpointBuilder.WithName(this.EndpointName);
    //}
}

public abstract partial class BeamOsModelResourceBaseEndpoint<TCommand, TBody, TResponse>
    : BeamOsBaseEndpoint<TCommand, TResponse>
    where TCommand : IModelResourceRequest<TBody>, new() { }
