using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;

public interface IModelRepository : IRepository<ModelId, Model>
{
    public Task<Model?> GetSingle(
        ModelId modelId,
        Func<IQueryable<Model>, IQueryable<Model>>? includeNavigationProperties = null,
        CancellationToken ct = default
    );

    public Task<Model?> GetSingle(
        ModelId modelId,
        CancellationToken ct = default,
        params string[] includeNavigationProperties
    );
}

public interface IModelProposalRepository
    : IModelResourceRepository<ModelProposalId, ModelProposal> { }

public interface IProposalIssueRepository
    : IModelResourceRepository<ProposalIssueId, ProposalIssue> { }

public sealed class InMemoryModelRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage,
    [FromKeyedServices("InMemory")] INodeRepository nodeRepository,
    [FromKeyedServices("InMemory")] IElement1dRepository element1dRepository,
    [FromKeyedServices("InMemory")] IMaterialRepository materialRepository,
    [FromKeyedServices("InMemory")] ISectionProfileRepository sectionProfileRepository
) : IModelRepository
{
    public void Add(Model aggregate)
    {
        inMemoryModelRepositoryStorage.Models.Add(aggregate.Id, aggregate);
    }

    public void ClearChangeTracker()
    {
        // No-op for in-memory repository
    }

    public async Task<Model?> GetSingle(
        ModelId modelId,
        Func<IQueryable<Model>, IQueryable<Model>>? includeNavigationProperties = null,
        CancellationToken ct = default
    )
    {
        var model = inMemoryModelRepositoryStorage.Models.GetValueOrDefault(modelId);
        if (model is null)
        {
            return null;
        }

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
    ) => this.GetSingle(modelId, null, ct);

    public void Put(Model aggregate)
    {
        if (inMemoryModelRepositoryStorage.Models.ContainsKey(aggregate.Id))
        {
            inMemoryModelRepositoryStorage.Models[aggregate.Id] = aggregate;
        }
        else
        {
            throw new KeyNotFoundException($"Model with ID {aggregate.Id} not found.");
        }
    }

    public void Remove(Model aggregate)
    {
        inMemoryModelRepositoryStorage.Models.Remove(aggregate.Id);
    }
}
