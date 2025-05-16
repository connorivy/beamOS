using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Infrastructure.Common;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Materials;

internal sealed class ModelProposalRepository(StructuralAnalysisDbContext dbContext)
    : ModelResourceRepositoryBase<ModelProposalId, ModelProposal>(dbContext),
        IModelProposalRepository { }
