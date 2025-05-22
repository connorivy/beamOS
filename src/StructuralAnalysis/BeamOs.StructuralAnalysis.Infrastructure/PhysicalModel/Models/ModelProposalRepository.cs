using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
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
            .DbContext.ModelProposals.AsSplitQuery()
            .Where(m => m.ModelId == modelId && m.Id == id)
            .Include(m => m.NodeProposals)
            .Include(m => m.Element1dProposals)
            .Include(m => m.ProposalIssues)
            .Include(m => m.MaterialProposals)
            .Include(m => m.SectionProfileProposals)
            .Include(m => m.SectionProfileProposalsFromLibrary)
            .FirstOrDefaultAsync(ct);
    }
}
