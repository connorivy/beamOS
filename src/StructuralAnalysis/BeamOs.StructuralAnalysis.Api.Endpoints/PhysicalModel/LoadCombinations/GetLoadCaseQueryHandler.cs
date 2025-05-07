using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinations;
using LoadCombination = BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations.LoadCombination;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.LoadCombinations;

public class GetLoadCombinationCommandHandler(ILoadCombinationRepository entityRepository)
    : GetModelEntityCommandHandler<
        LoadCombinationId,
        Domain.PhysicalModel.LoadCombinations.LoadCombination,
        LoadCombination
    >(entityRepository)
{
    protected override LoadCombination MapToResponse(
        Domain.PhysicalModel.LoadCombinations.LoadCombination entity
    ) => entity.ToResponse();
}
