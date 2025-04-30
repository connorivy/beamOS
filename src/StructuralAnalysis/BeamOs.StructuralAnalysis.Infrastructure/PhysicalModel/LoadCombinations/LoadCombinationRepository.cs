using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinationAggregate;
using BeamOs.StructuralAnalysis.Infrastructure.Common;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.LoadCombinations;

internal class LoadCombinationRepository(StructuralAnalysisDbContext dbContext)
    : ModelResourceRepositoryBase<LoadCombinationId, LoadCombination>(dbContext),
        ILoadCombinationRepository { }
