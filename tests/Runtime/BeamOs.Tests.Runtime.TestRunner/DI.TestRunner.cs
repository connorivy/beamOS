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
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
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

        _ = services.AddKeyedScoped<INodeRepository, InMemoryNodeRepository>("InMemory");
        _ = services.AddKeyedScoped<IModelRepository, InMemoryModelRepository>("InMemory");
        _ = services.AddKeyedScoped<IMaterialRepository, InMemoryMaterialRepository>("InMemory");
        _ = services.AddKeyedScoped<ISectionProfileRepository, InMemorySectionProfileRepository>(
            "InMemory"
        );
        _ = services.AddKeyedScoped<
            ISectionProfileFromLibraryRepository,
            InMemorySectionProfileFromLibraryRepository
        >("InMemory");
        _ = services.AddKeyedScoped<IElement1dRepository, InMemoryElement1dRepository>("InMemory");
        _ = services.AddScoped<IPointLoadRepository, InMemoryPointLoadRepository>();
        _ = services.AddScoped<IMomentLoadRepository, InMemoryMomentLoadRepository>();
        _ = services.AddScoped<ILoadCaseRepository, InMemoryLoadCaseRepository>();
        _ = services.AddScoped<ILoadCombinationRepository, InMemoryLoadCombinationRepository>();
        _ = services.AddScoped<INodeResultRepository, InMemoryNodeResultRepository>();
        _ = services.AddScoped<IResultSetRepository, InMemoryResultSetRepository>();
        // _ = services.AddScoped<IEnvelopeResultSetRepository, InMemoryEnvelopeResultSetRepository>();
        // _ = services.AddScoped<IModelProposalRepository, InMemoryModelProposalRepository>();
        // _ = services.AddScoped<IProposalIssueRepository, InMemoryProposalIssueRepository>();

        return services;
    }
}
