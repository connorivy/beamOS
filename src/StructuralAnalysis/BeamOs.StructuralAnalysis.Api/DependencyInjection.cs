using System.Reflection;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Api;
using BeamOs.StructuralAnalysis.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace BeamOs.StructuralAnalysis.Api;

public static class DependencyInjection
{
    public static void MapEndpoints<TAssemblyMarker>(this IEndpointRouteBuilder app)
    {
        var endpointGroup = app.MapGroup("api");

        static MethodInfo mapMethodFactory() => typeof(EndpointToMinimalApi).GetMethod("Map");
        IEnumerable<Type> assemblyTypes = typeof(TAssemblyMarker)
            .Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract);

        var baseType = typeof(BeamOsActualBaseEndpoint<,>);
        foreach (var assemblyType in assemblyTypes)
        {
            if (
                Common.Application.DependencyInjection.GetConcreteBaseType(assemblyType, baseType)
                is Type endpointType
            )
            {
                var requestType = endpointType.GenericTypeArguments.FirstOrDefault();
                var responseType = endpointType.GenericTypeArguments.LastOrDefault();
                if (requestType is null || responseType is null)
                {
                    throw new Exception(
                        $"request or response type of assembly type {assemblyType} was null"
                    );
                }

                mapMethodFactory()
                    .MakeGenericMethod([assemblyType, requestType, responseType])
                    .Invoke(null, [endpointGroup]);
            }
        }

        //EndpointToMinimalApi.Map<CreateNode, CreateNodeCommand, NodeResponse>(endpointGroup);
        //EndpointToMinimalApi.Map<UpdateNode, PatchNodeCommand, NodeResponse>(endpointGroup);
        //EndpointToMinimalApi.Map<CreateModel, CreateModelRequest, ModelResponse>(endpointGroup);
    }

    public static async Task InitializeBeamOsDb(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<StructuralAnalysisDbContext>();

            //await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();
        }
    }
}

public static class EndpointToMinimalApi
{
    public static void Map<TEndpoint, TRequest, TResponse>(IEndpointRouteBuilder app)
        where TEndpoint : BeamOsActualBaseEndpoint<TRequest, TResponse>
    {
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
        var tags = typeof(TEndpoint).GetCustomAttributes<BeamOsTagAttribute>();

        Func<string, Delegate, IEndpointConventionBuilder> mapFunc = endpointType switch
        {
            Http.Delete => app.MapDelete,
            Http.Get => app.MapGet,
            Http.Patch => app.MapPatch,
            Http.Post => app.MapPost,
            Http.Put => app.MapPut,
            _ => throw new NotImplementedException(),
        };

        Delegate mapDelegate;
        if (
            Common.Application.DependencyInjection.ConcreteTypeDerivedFromBase(
                typeof(TEndpoint),
                typeof(BeamOsFromBodyResultBaseEndpoint<,>)
            )
            || Common.Application.DependencyInjection.ConcreteTypeDerivedFromBase(
                typeof(TEndpoint),
                typeof(BeamOsFromBodyBaseEndpoint<,>)
            )
        )
        {
            mapDelegate = ([FromBody] TRequest req, IServiceProvider serviceProvider) =>
                serviceProvider.GetRequiredService<TEndpoint>().ExecuteRequestAsync(req);
        }
        else
        {
            mapDelegate = ([AsParameters] TRequest req, IServiceProvider serviceProvider) =>
                serviceProvider.GetRequiredService<TEndpoint>().ExecuteRequestAsync(req);
        }

        IEndpointConventionBuilder endpointBuilder = mapFunc(route, mapDelegate);

        endpointBuilder.WithName(typeof(TEndpoint).Name);
        foreach (var tag in tags)
        {
            endpointBuilder.WithTags(tag.Value);
        }
    }
}
