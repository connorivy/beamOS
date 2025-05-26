using BeamOs.StructuralAnalysis.Application.AnalyticalResults.NodeResults;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
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
        // _ = services.AddScoped<IPointLoadRepository, PointLoadRepository>();
        // _ = services.AddScoped<IMomentLoadRepository, MomentLoadRepository>();
        // _ = services.AddScoped<ILoadCaseRepository, LoadCaseRepository>();
        // _ = services.AddScoped<ILoadCombinationRepository, LoadCombinationRepository>();
        // _ = services.AddScoped<INodeResultRepository, NodeResultRepository>();
        // _ = services.AddScoped<IResultSetRepository, ResultSetRepository>();
        // _ = services.AddScoped<IEnvelopeResultSetRepository, EnvelopeResultSetRepository>();
        // _ = services.AddScoped<IModelProposalRepository, ModelProposalRepository>();
        // _ = services.AddScoped<IProposalIssueRepository, ProposalIssueRepository>();

        return services;
    }
}

public class InMemoryModelRepository(
    INodeRepository nodeRepository,
    IElement1dRepository element1dRepository,
    IMaterialRepository materialRepository,
    ISectionProfileRepository sectionProfileRepository
) : InMemoryRepository<ModelId, Model>, IModelRepository
{
    public async Task<Model?> GetSingle(
        ModelId modelId,
        Func<IQueryable<Model>, IQueryable<Model>>? includeNavigationProperties = null,
        CancellationToken ct = default
    )
    {
        var model = this.Entities[modelId];
        model.Nodes = await nodeRepository.GetMany(modelId, null, ct);
        foreach (var el in await element1dRepository.GetMany(modelId, null, ct))
        {
            model.Element1ds.Add(el);
        }
        foreach (var el in await materialRepository.GetMany(modelId, null, ct))
        {
            model.Materials.Add(el);
        }
        foreach (var el in await sectionProfileRepository.GetMany(modelId, null, ct))
        {
            model.SectionProfiles.Add(el);
        }

        return model;
    }

    public Task<Model?> GetSingle(
        ModelId modelId,
        CancellationToken ct = default,
        params string[] includeNavigationProperties
    ) => GetSingle(modelId, null, ct);
}

public class InMemoryElement1dRepository
    : InMemoryModelResourceRepository<Element1dId, Element1d>,
        IElement1dRepository
{
    public Task<List<Element1d>> GetMany(
        ModelId modelId,
        IList<Element1dId>? ids,
        CancellationToken ct = default
    )
    {
        return Task.FromResult(ids is null ? [] : ids.Select(x => this.Entities[x]).ToList());
    }
}

public class InMemoryNodeResultRepository
    : InMemoryAnalyticalResultRepository<NodeId, NodeResult>,
        INodeResultRepository { }

public class InMemoryMaterialRepository
    : InMemoryModelResourceRepository<MaterialId, Material>,
        IMaterialRepository { }

public class InMemorySectionProfileRepository
    : InMemoryModelResourceRepository<SectionProfileId, SectionProfile>,
        ISectionProfileRepository { }

public class InMemorySectionProfileFromLibraryRepository
    : InMemoryModelResourceRepository<SectionProfileId, SectionProfileFromLibrary>,
        ISectionProfileFromLibraryRepository { }
