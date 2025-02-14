using BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Infrastructure.Common;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Materials;

internal class MaterialRepository(StructuralAnalysisDbContext dbContext)
    : ModelResourceRepositoryBase<MaterialId, Material>(dbContext),
        IMaterialRepository { }
