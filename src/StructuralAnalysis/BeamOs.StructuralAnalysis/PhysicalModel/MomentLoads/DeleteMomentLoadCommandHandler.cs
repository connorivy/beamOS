using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads;

internal class DeleteMomentLoadCommandHandler(
    IMomentLoadRepository repo,
    IStructuralAnalysisUnitOfWork unitOfWork
) : DeleteModelEntityCommandHandler<MomentLoadId, MomentLoad>(repo, unitOfWork) { }
