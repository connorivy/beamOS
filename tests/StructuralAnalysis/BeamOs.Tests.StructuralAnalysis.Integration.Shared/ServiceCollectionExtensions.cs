using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTestServices(this IServiceCollection services)
    {
        // services.AddScoped<OpenSeesTests>();
        services.AddScoped<DsmElement1dTests>();
        services.AddScoped<DsmModelTests>();
        services.AddScoped<TestInfoRetriever>();

        services.AddSingleton<ISolverFactory, CholeskySolverFactory>();
        services.AddInMemoryCommandHandlers();
        services.AddKeyedScoped<IStructuralAnalysisApiClientV1, InMemoryApiClient>("InMemory");
        services.AddScoped<InMemoryModelRepositoryStorage>();
        services.AddKeyedScoped<IStructuralAnalysisUnitOfWork, InMemoryUnitOfWork>("InMemory");
        services.AddKeyedScoped<INodeRepository, InMemoryNodeRepository>("InMemory");
        services.AddKeyedScoped<IModelRepository, InMemoryModelRepository>("InMemory");
        services.AddKeyedScoped<IMaterialRepository, InMemoryMaterialRepository>("InMemory");
        services.AddKeyedScoped<ISectionProfileRepository, InMemorySectionProfileRepository>(
            "InMemory"
        );
        services.AddKeyedScoped<
            ISectionProfileFromLibraryRepository,
            InMemorySectionProfileFromLibraryRepository
        >("InMemory");
        services.AddKeyedScoped<IElement1dRepository, InMemoryElement1dRepository>("InMemory");
        services.AddKeyedScoped<IPointLoadRepository, InMemoryPointLoadRepository>("InMemory");
        services.AddKeyedScoped<IMomentLoadRepository, InMemoryMomentLoadRepository>("InMemory");
        services.AddKeyedScoped<ILoadCaseRepository, InMemoryLoadCaseRepository>("InMemory");
        services.AddKeyedScoped<ILoadCombinationRepository, InMemoryLoadCombinationRepository>(
            "InMemory"
        );
        services.AddKeyedScoped<INodeResultRepository, InMemoryNodeResultRepository>("InMemory");
        services.AddKeyedScoped<IResultSetRepository, InMemoryResultSetRepository>("InMemory");
        services.AddKeyedScoped<IEnvelopeResultSetRepository, InMemoryEnvelopeResultSetRepository>(
            "InMemory"
        );
        services.AddKeyedScoped<IModelProposalRepository, InMemoryModelProposalRepository>(
            "InMemory"
        );
        services.AddKeyedScoped<IProposalIssueRepository, InMemoryProposalIssueRepository>(
            "InMemory"
        );
        services.AddKeyedScoped<
            IQueryHandler<EmptyRequest, List<ModelInfoResponse>>,
            InMemoryGetModelsQueryHandler
        >("InMemory");

        return services;
    }
}
