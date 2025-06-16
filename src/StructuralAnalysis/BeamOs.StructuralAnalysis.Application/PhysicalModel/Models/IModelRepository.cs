using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
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

public interface IModelProposalRepository : IModelResourceRepository<ModelProposalId, ModelProposal>
{
    public Task<ModelProposal?> GetSingle(
        ModelId modelId,
        ModelProposalId id,
        IReadOnlyList<IEntityProposal>? proposalsToIgnore,
        CancellationToken ct = default
    );
}

public sealed class InMemoryModelProposalRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
)
    : InMemoryModelResourceRepository<ModelProposalId, ModelProposal>(
        inMemoryModelRepositoryStorage
    ),
        IModelProposalRepository
{
    public Task<ModelProposal?> GetSingle(
        ModelId modelId,
        ModelProposalId id,
        IReadOnlyList<IEntityProposal>? proposalsToIgnore,
        CancellationToken ct = default
    )
    {
        if (proposalsToIgnore is null)
        {
            return this.GetSingle(modelId, id, ct);
        }

        if (
            !this.ModelResources.TryGetValue(modelId, out var resources)
            || resources.TryGetValue(id, out var modelProposal)
        )
        {
            return Task.FromResult(default(ModelProposal));
        }

        // Filter out ignored proposals
        modelProposal.NodeProposals = modelProposal
            .NodeProposals.Where(p =>
                !proposalsToIgnore.Any(i => i.Id == p.Id && i.ObjectType == BeamOsObjectType.Node)
            )
            .ToList();
        modelProposal.Element1dProposals = modelProposal
            .Element1dProposals.Where(p =>
                !proposalsToIgnore.Any(i =>
                    i.Id == p.Id && i.ObjectType == BeamOsObjectType.Element1d
                )
            )
            .ToList();
        modelProposal.MaterialProposals = modelProposal
            .MaterialProposals.Where(p =>
                !proposalsToIgnore.Any(i =>
                    i.Id == p.Id && i.ObjectType == BeamOsObjectType.Material
                )
            )
            .ToList();
        modelProposal.SectionProfileProposals = modelProposal
            .SectionProfileProposals.Where(p =>
                !proposalsToIgnore.Any(i =>
                    i.Id == p.Id && i.ObjectType == BeamOsObjectType.SectionProfile
                )
            )
            .ToList();
        modelProposal.SectionProfileProposalsFromLibrary = modelProposal
            .SectionProfileProposalsFromLibrary.Where(p =>
                !proposalsToIgnore.Any(i =>
                    i.Id == p.Id && i.ObjectType == BeamOsObjectType.SectionProfile
                )
            )
            .ToList();

        return Task.FromResult(modelProposal);
    }
}

public interface IProposalIssueRepository
    : IModelResourceRepository<ProposalIssueId, ProposalIssue> { }

public sealed class InMemoryProposalIssueRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
)
    : InMemoryModelResourceRepository<ProposalIssueId, ProposalIssue>(
        inMemoryModelRepositoryStorage
    ),
        IProposalIssueRepository { }

public sealed class InMemoryModelRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage,
    [FromKeyedServices("InMemory")] INodeRepository nodeRepository,
    [FromKeyedServices("InMemory")] IInternalNodeRepository internalNodeRepository,
    [FromKeyedServices("InMemory")] IElement1dRepository element1dRepository,
    [FromKeyedServices("InMemory")] IMaterialRepository materialRepository,
    [FromKeyedServices("InMemory")] ISectionProfileRepository sectionProfileRepository,
    [FromKeyedServices("InMemory")]
        ISectionProfileFromLibraryRepository sectionProfileFromLibraryRepository
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
        model.InternalNodes = await internalNodeRepository.GetMany(modelId, null, ct);
        model.Element1ds = await element1dRepository.GetMany(
            modelId,
            default(IList<Element1dId>),
            ct
        );
        model.Materials = await materialRepository.GetMany(modelId, null, ct);
        model.SectionProfiles = await sectionProfileRepository.GetMany(modelId, null, ct);
        model.SectionProfilesFromLibrary = await sectionProfileFromLibraryRepository.GetMany(
            modelId,
            null,
            ct
        );

        return model;
    }

    public Task<Model?> GetSingle(
        ModelId modelId,
        CancellationToken ct = default,
        params string[] includeNavigationProperties
    ) => this.GetSingle(modelId, null, ct);

    public ValueTask Put(Model aggregate)
    {
        if (inMemoryModelRepositoryStorage.Models.ContainsKey(aggregate.Id))
        {
            inMemoryModelRepositoryStorage.Models[aggregate.Id] = aggregate;
            return ValueTask.CompletedTask;
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
