using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Api.Endpoints;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.EnvelopeResultSets;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.NodeResults;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.ResultSets;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod;
using BeamOs.Tests.StructuralAnalysis.Unit.DirectStiffnessMethod;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.Tests.Runtime.TestRunner;

public static class DI
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
        services.AddKeyedScoped<IModelRepository, InMemoryModelRepository>("InMemory");
        services.AddKeyedScoped<INodeDefinitionRepository, InMemoryNodeDefinitionRepository>(
            "InMemory"
        );
        services.AddKeyedScoped<INodeRepository, InMemoryNodeRepository>("InMemory");
        services.AddKeyedScoped<IInternalNodeRepository, InMemoryInternalNodeRepository>(
            "InMemory"
        );
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

        services.AddKeyedScoped<
            ICommandHandler<ModelResourceRequest<DateTimeOffset>, ModelResponse>,
            InMemoryRestoreModeCommandHandler
        >("InMemory");

        return services;
    }
}
