using System.Reflection;
using BeamOs.Common.Api;
using BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;

namespace BeamOs.StructuralAnalysis.Api;

public static class DependencyInjection
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpointGroup = app.MapGroup("api");
        EndpointToMinimalApi.Map<CreateNode, CreateNodeCommand, NodeResponse>(endpointGroup);
        EndpointToMinimalApi.Map<UpdateNode, PatchNodeCommand, NodeResponse>(endpointGroup);
        EndpointToMinimalApi.Map<CreateModel, CreateModelRequest, ModelResponse>(endpointGroup);
    }
}

public static class EndpointToMinimalApi
{
    public static void Map<TEndpoint, TRequest, TResponse>(IEndpointRouteBuilder app)
        where TEndpoint : BeamOsBaseEndpoint<TRequest, TResponse>
    {
        IEndpointConventionBuilder endpointBuilder;

        string route =
            typeof(TEndpoint).GetCustomAttribute<BeamOsRouteAttribute>()?.Value
            ?? throw new InvalidOperationException(
                $"Class {typeof(TEndpoint).Name} is missing the route attribute"
            );
        string endpointType =
            typeof(TEndpoint).GetCustomAttribute<BeamOsEndpointTypeAttribute>()?.Value
            ?? throw new InvalidOperationException(
                $"Class {typeof(TEndpoint).Name} is missing the route attribute"
            );

        if (endpointType is Http.Post)
        {
            endpointBuilder = app.MapPost(
                route,
                async ([AsParameters] TRequest req, IServiceProvider serviceProvider) =>
                    await serviceProvider.GetRequiredService<TEndpoint>().ExecuteRequestAsync(req)
            );
        }
        else if (endpointType is Http.Get)
        {
            endpointBuilder = app.MapGet(
                route,
                async ([AsParameters] TRequest req, IServiceProvider serviceProvider) =>
                    await serviceProvider.GetRequiredService<TEndpoint>().ExecuteRequestAsync(req)
            );
        }
        else if (endpointType is Http.Patch)
        {
            endpointBuilder = app.MapPatch(
                route,
                async ([AsParameters] TRequest req, IServiceProvider serviceProvider) =>
                    await serviceProvider.GetRequiredService<TEndpoint>().ExecuteRequestAsync(req)
            );
        }
        else if (endpointType is Http.Put)
        {
            endpointBuilder = app.MapPut(
                route,
                async ([AsParameters] TRequest req, IServiceProvider serviceProvider) =>
                    await serviceProvider.GetRequiredService<TEndpoint>().ExecuteRequestAsync(req)
            );
        }
        else
        {
            throw new NotImplementedException();
        }

        endpointBuilder.WithName(typeof(TEndpoint).Name);
    }
}
