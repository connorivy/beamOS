using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.LoadCases;

public class GetLoadCaseCommandHandler(ILoadCaseRepository entityRepository) : GetModelEntityCommandHandler<LoadCaseId, LoadCase, LoadCaseResponse>(entityRepository)
{
    protected override LoadCaseResponse MapToResponse(LoadCase entity) => entity.ToResponse();
}
