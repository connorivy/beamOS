using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinations;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;

public sealed class DeleteLoadCombinationCommandHandler(
    ILoadCombinationRepository entityRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : DeleteModelEntityCommandHandler<LoadCombinationId, LoadCombination>(
        entityRepository,
        unitOfWork
    ) { }
