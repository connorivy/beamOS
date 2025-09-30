using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads;

internal class DeletePointLoadCommandHandler(
    IPointLoadRepository repo,
    IStructuralAnalysisUnitOfWork unitOfWork
) : DeleteModelEntityCommandHandler<PointLoadId, PointLoad>(repo, unitOfWork) { }
