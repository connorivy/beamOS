using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;

internal sealed class DeleteLoadCaseCommandHandler(
    ILoadCaseRepository entityRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : DeleteModelEntityCommandHandler<LoadCaseId, Domain.PhysicalModel.LoadCases.LoadCase>(
        entityRepository,
        unitOfWork
    ) { }
