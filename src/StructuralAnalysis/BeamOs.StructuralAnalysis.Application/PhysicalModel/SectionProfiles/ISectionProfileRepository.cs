using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;

public interface ISectionProfileRepository
    : IModelResourceRepository<SectionProfileId, SectionProfile> { }

public class InMemorySectionProfileRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
)
    : InMemoryModelResourceRepository<SectionProfileId, SectionProfile>(
        inMemoryModelRepositoryStorage
    ),
        ISectionProfileRepository { }
