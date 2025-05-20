using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Infrastructure.Common;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Models;

internal sealed class ProposalIssueRepository(StructuralAnalysisDbContext dbContext)
    : ModelResourceRepositoryBase<ProposalIssueId, ProposalIssue>(dbContext),
        IProposalIssueRepository { }
