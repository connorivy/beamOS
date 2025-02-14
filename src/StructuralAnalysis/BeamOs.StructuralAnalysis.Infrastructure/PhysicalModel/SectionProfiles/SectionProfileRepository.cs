using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using BeamOs.StructuralAnalysis.Infrastructure.Common;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.SectionProfiles;

internal class SectionProfileRepository(StructuralAnalysisDbContext dbContext)
    : ModelResourceRepositoryBase<SectionProfileId, SectionProfile>(dbContext),
        ISectionProfileRepository { }
