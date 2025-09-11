using System.Reflection;
using BeamOs.Common.Api;
using BeamOs.StructuralAnalysis.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.StructuralAnalysis.Api;

public static partial class DependencyInjection
{
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
            var dbContext = scope.ServiceProvider.GetRequiredService<StructuralAnalysisDbContext>();

            // await dbContext.Database.EnsureDeletedAsync();
            var appliedMigrations = (await dbContext.Database.GetAppliedMigrationsAsync()).ToList();
            var pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync()).ToList();
            if (pendingMigrations.Count > 0)
            {
                await dbContext.Database.MigrateAsync();
            }
            // await dbContext.Database.EnsureCreatedAsync();
        }
    }
}
