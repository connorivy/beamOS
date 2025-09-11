using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinations;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;

public class GetLoadCombinationCommandHandler(ILoadCombinationRepository entityRepository)
    : GetModelEntityCommandHandler<LoadCombinationId, LoadCombination, LoadCombinationContract>(
        entityRepository
    )
{
    protected override LoadCombinationContract MapToResponse(LoadCombination entity) =>
        entity.ToResponse();
}
