using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;

internal interface ISectionProfileRepository
    : IModelResourceRepository<SectionProfileId, SectionProfile> { }

internal class InMemorySectionProfileRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
)
    : InMemoryModelResourceRepository<SectionProfileId, SectionProfile>(
        inMemoryModelRepositoryStorage
    ),
        ISectionProfileRepository { }
