using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Models;

internal sealed class ModelProposalRepository(StructuralAnalysisDbContext dbContext)
    : ModelResourceRepositoryBase<ModelProposalId, ModelProposal>(dbContext),
        IModelProposalRepository
{
    public override Task<ModelProposal?> GetSingle(
        ModelId modelId,
        ModelProposalId id,
        CancellationToken ct = default
    )
    {
        return this
            .DbContext.ModelProposals.AsNoTracking()
            .AsSplitQuery()
            .Where(m => m.ModelId == modelId && m.Id == id)
            .Include(m => m.NodeProposals)
            .Include(m => m.InternalNodeProposals)
            .Include(m => m.Element1dProposals)
            .Include(m => m.ProposalIssues)
            .Include(m => m.MaterialProposals)
            .Include(m => m.SectionProfileProposals)
            .Include(m => m.SectionProfileProposalsFromLibrary)
            .Include(m => m.DeleteModelEntityProposals)
            .FirstOrDefaultAsync(ct);
    }

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

        var nodeProposalsToIgnore = proposalsToIgnore
            .Where(p =>
                p.ObjectType == BeamOsObjectType.Node && p.ProposalType != ProposalType.Delete
            )
            .Select(p => p.Id)
            .ToList();

        var internalNodeProposalsToIgnore = proposalsToIgnore
            .Where(p =>
                p.ObjectType == BeamOsObjectType.InternalNode
                && p.ProposalType != ProposalType.Delete
            )
            .Select(p => p.Id)
            .ToList();

        var element1dProposalsToIgnore = proposalsToIgnore
            .Where(p =>
                p.ObjectType == BeamOsObjectType.Element1d && p.ProposalType != ProposalType.Delete
            )
            .Select(p => p.Id)
            .ToList();
        var materialProposalsToIgnore = proposalsToIgnore
            .Where(p =>
                p.ObjectType == BeamOsObjectType.Material && p.ProposalType != ProposalType.Delete
            )
            .Select(p => p.Id)
            .ToList();
        var sectionProfileProposalsToIgnore = proposalsToIgnore
            .Where(p =>
                p.ObjectType == BeamOsObjectType.SectionProfile
                && p.ProposalType != ProposalType.Delete
            )
            .Select(p => p.Id)
            .ToList();
        var deleteModelEntityProposalsToIgnore = proposalsToIgnore
            .Where(p => p.ProposalType == ProposalType.Delete)
            .Select(p => p.Id)
            .ToList();

        return this
            .DbContext.ModelProposals.AsNoTracking()
            .AsSplitQuery()
            .Where(m => m.ModelId == modelId && m.Id == id)
            .Include(m => m.NodeProposals.Where(np => !nodeProposalsToIgnore.Contains(np.Id)))
            .Include(m =>
                m.InternalNodeProposals.Where(inp =>
                    !internalNodeProposalsToIgnore.Contains(inp.Id)
                )
            )
            .Include(m =>
                m.Element1dProposals.Where(ep => !element1dProposalsToIgnore.Contains(ep.Id))
            )
            .Include(m => m.ProposalIssues)
            .Include(m =>
                m.MaterialProposals.Where(mp => !materialProposalsToIgnore.Contains(mp.Id))
            )
            .Include(m =>
                m.SectionProfileProposals.Where(sp =>
                    !sectionProfileProposalsToIgnore.Contains(sp.Id)
                )
            )
            .Include(m =>
                m.SectionProfileProposalsFromLibrary.Where(sp =>
                    !sectionProfileProposalsToIgnore.Contains(sp.Id)
                )
            )
            .Include(m =>
                m.DeleteModelEntityProposals.Where(m =>
                    !deleteModelEntityProposalsToIgnore.Contains(m.Id)
                )
            )
            .FirstOrDefaultAsync(ct);
    }
}
