using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;

public class GetLoadCaseCommandHandler(ILoadCaseRepository entityRepository)
    : GetModelEntityCommandHandler<LoadCaseId, LoadCase, LoadCaseContract>(entityRepository)
{
    protected override LoadCaseContract MapToResponse(LoadCase entity) => entity.ToResponse();
}
