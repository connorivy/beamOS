using System.Reflection;
using BeamOs.Common.Api;
using BeamOs.StructuralAnalysis.Infrastructure;
using BeamOs.StructuralAnalysis.Sdk;
using Microsoft.EntityFrameworkCore;
using ServiceScan.SourceGenerator;

namespace BeamOs.StructuralAnalysis.Api;

public static partial class DependencyInjection
{
    [GenerateServiceRegistrations(
        AssignableTo = typeof(BeamOsBaseEndpoint<,>),
        FromAssemblyOf = typeof(BeamOs.StructuralAnalysis.Application.DependencyInjection),
        // AttributeFilter = typeof(BeamOsRouteAttribute),
        CustomHandler = nameof(Asdf)
    )]
    public static partial IEndpointRouteBuilder MapStructuralEndpoints(
        this IEndpointRouteBuilder services
    );

    [GenerateServiceRegistrations(
        AssignableTo = typeof(BeamOsBaseEndpoint<,>),
        FromAssemblyOf = typeof(BeamOs.SpeckleConnector.IAssemblyMarkerSpeckleConnector),
        CustomHandler = nameof(Asdf)
    )]
    public static partial IEndpointRouteBuilder MapSpeckleEndpoints(
        this IEndpointRouteBuilder services
    );

    // [GenerateServiceRegistrations(
    //     AssignableTo = typeof(BeamOsBaseEndpoint<,>),
    //     FromAssemblyOf = typeof(BeamOs.Ai.IAssemblyMarkerAi),
    //     CustomHandler = nameof(Asdf)
    // )]
    // public static partial IEndpointRouteBuilder MapAiEndpoints(this IEndpointRouteBuilder services);

    private static void Asdf<TEndpoint, TRequest, TResponse>(IEndpointRouteBuilder app)
        where TEndpoint : BeamOsBaseEndpoint<TRequest, TResponse>
    // where T : BeamOsBaseEndpoint<TRequest, TResponse>
    {
        EndpointToMinimalApi.Map<TEndpoint, TRequest, TResponse>(app);
    }

    public static void MapEndpoints<TAssemblyMarker>(this IEndpointRouteBuilder app)
    {
        var endpointGroup = app.MapGroup("api");

        static MethodInfo mapMethodFactory() => typeof(EndpointToMinimalApi).GetMethod("Map");
        IEnumerable<Type> assemblyTypes = typeof(TAssemblyMarker)
            .Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract);

        var baseType = typeof(BeamOsBaseEndpoint<,>);
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
            await BeamOs.StructuralAnalysis.Infrastructure.DependencyInjection.MigrateDb(scope);

            var apiClient = scope.ServiceProvider.GetRequiredService<BeamOsResultApiClient>();
            foreach (var model in Tests.Common.AllSolvedProblems.ModelFixtures())
            {
                if (await model.CreateOnly(apiClient))
                {
                    await apiClient
                        .Models[model.Id]
                        .Analyze.Opensees.RunOpenSeesAnalysisAsync(new());
                }
            }
        }
    }
}
