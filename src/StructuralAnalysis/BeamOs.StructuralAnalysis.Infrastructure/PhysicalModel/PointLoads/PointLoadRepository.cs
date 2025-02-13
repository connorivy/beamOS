using BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.StructuralAnalysis.Infrastructure.Common;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.PointLoads;

internal class PointLoadRepository(StructuralAnalysisDbContext dbContext)
    : ModelResourceRepositoryBase<PointLoadId, PointLoad>(dbContext),
        IPointLoadRepository { }
