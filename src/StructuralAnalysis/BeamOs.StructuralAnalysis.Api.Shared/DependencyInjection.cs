using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Application;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.StructuralAnalysis.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddStructuralAnalysis(
        this IServiceCollection services,
        string connectionString
    ) =>
        services
            .AddStructuralAnalysisApplication()
            .AddStructuralAnalysisInfrastructure(connectionString)
            .AddStructuralAnalysisApi();

    public static IServiceCollection AddStructuralAnalysisApi(this IServiceCollection services)
    {
        return services.AddObjectThatImplementInterface<IAssemblyMarkerStructuralAnalysisApiShared>(
            typeof(IBaseEndpoint<,>),
            ServiceLifetime.Scoped,
            false
        );
    }

    public static void MapEndpoints(this WebApplication app)
    {
        var endpointGroup = app.MapGroup("api");
        EndpointToMinimalApi.Map<CreateNode, CreateNodeCommand, NodeResponse>(endpointGroup);
        EndpointToMinimalApi.Map<UpdateNode, PatchNodeCommand, NodeResponse>(endpointGroup);
    }
}
