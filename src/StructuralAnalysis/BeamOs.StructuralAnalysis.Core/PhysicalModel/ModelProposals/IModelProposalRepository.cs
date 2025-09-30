using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;

internal interface IModelProposalRepository
    : IModelResourceRepository<ModelProposalId, ModelProposal>
{
    public Task<ModelProposal?> GetSingle(
        ModelId modelId,
        ModelProposalId id,
        IReadOnlyList<IEntityProposal>? proposalsToIgnore,
        CancellationToken ct = default
    );
}

internal sealed class InMemoryModelProposalRepository(
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
        modelProposal.ProposalIssues = modelProposal
            .ProposalIssues.Where(p =>
                !proposalsToIgnore.Any(i => i.Id == p.Id && i.ProposalType == ProposalType.Delete)
            )
            .ToList();

        return Task.FromResult(modelProposal);
    }

    public override async Task<ModelProposal?> GetSingle(
        ModelId modelId,
        ModelProposalId id,
        CancellationToken ct = default
    )
    {
        var modelProposal = await base.GetSingle(modelId, id, ct);
        modelProposal.ProposalIssues ??= [];
        modelProposal.MaterialProposals ??= [];
        modelProposal.SectionProfileProposals ??= [];
        modelProposal.SectionProfileProposalsFromLibrary ??= [];
        modelProposal.NodeProposals ??= [];
        modelProposal.InternalNodeProposals ??= [];
        modelProposal.Element1dProposals ??= [];
        modelProposal.DeleteModelEntityProposals ??= [];
        return modelProposal;
    }
}

internal interface IProposalIssueRepository
    : IModelResourceRepository<ProposalIssueId, ProposalIssue> { }

internal sealed class InMemoryProposalIssueRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
)
    : InMemoryModelResourceRepository<ProposalIssueId, ProposalIssue>(
        inMemoryModelRepositoryStorage
    ),
        IProposalIssueRepository { }
