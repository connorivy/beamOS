using System.Reflection;
using BeamOs.Common.Api;

namespace BeamOs.StructuralAnalysis.Api;

public static class EndpointToMinimalApi
{
    public static void Map<TEndpoint, TRequest, TResponse>(IEndpointRouteBuilder app)
        where TEndpoint : BeamOsBaseEndpoint<TRequest, TResponse>
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

        Func<string, Delegate, RouteHandlerBuilder> mapFunc = endpointType switch
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
        // || Common.Application.DependencyInjection.ConcreteTypeDerivedFromBase(
        //     typeof(TEndpoint),
        //     typeof(BeamOsFromBodyBaseEndpoint<,>)
        // )
        )
        {
            mapDelegate = async (
                [Microsoft.AspNetCore.Mvc.FromBody] TRequest req,
                IServiceProvider serviceProvider
            ) =>
            {
                var endpoint = serviceProvider.GetRequiredService<TEndpoint>();
#if CODEGEN
                return await endpoint.GetResponseTypeForClientGenerationPurposes();
#else
                return (await endpoint.ExecuteAsync(req)).ToWebResult();
#endif
            };
        }
        else
        {
            mapDelegate = async ([AsParameters] TRequest req, IServiceProvider serviceProvider) =>
            {
                var endpoint = serviceProvider.GetRequiredService<TEndpoint>();
#if CODEGEN
                return await endpoint.GetResponseTypeForClientGenerationPurposes();
#else
                return (await endpoint.ExecuteAsync(req)).ToWebResult();
#endif
            };
        }

        var endpointBuilder = mapFunc(route, mapDelegate);

        endpointBuilder.WithName(typeof(TEndpoint).Name);
        foreach (var tag in tags)
        {
            endpointBuilder.WithTags(tag.Value);
        }

        // endpointBuilder.ProducesProblem(404);
    }
}
