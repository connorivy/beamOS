using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.LoadCases;

public class GetLoadCaseCommandHandler(ILoadCaseRepository entityRepository)
    : GetModelEntityCommandHandler<LoadCaseId, Domain.PhysicalModel.LoadCases.LoadCase, LoadCase>(
        entityRepository
    )
{
    protected override LoadCase MapToResponse(Domain.PhysicalModel.LoadCases.LoadCase entity) =>
        entity.ToResponse();
}
