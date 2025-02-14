using BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.StructuralAnalysis.Infrastructure.Common;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.MomentLoads;

internal class MomentLoadRepository(StructuralAnalysisDbContext dbContext)
    : ModelResourceRepositoryBase<MomentLoadId, MomentLoad>(dbContext),
        IMomentLoadRepository { }
