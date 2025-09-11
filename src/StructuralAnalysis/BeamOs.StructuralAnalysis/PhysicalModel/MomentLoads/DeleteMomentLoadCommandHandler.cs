using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads;

public class DeleteMomentLoadCommandHandler(
    IMomentLoadRepository repo,
    IStructuralAnalysisUnitOfWork unitOfWork
) : DeleteModelEntityCommandHandler<MomentLoadId, MomentLoad>(repo, unitOfWork) { }
