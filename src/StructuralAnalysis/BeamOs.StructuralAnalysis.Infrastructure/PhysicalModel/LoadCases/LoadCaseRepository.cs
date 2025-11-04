using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Infrastructure.Common;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.LoadCases;

internal class LoadCaseRepository(StructuralAnalysisDbContext dbContext)
    : ModelResourceRepositoryBase<LoadCaseId, LoadCase>(dbContext),
        ILoadCaseRepository { }
