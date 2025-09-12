using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;

internal interface ISectionProfileFromLibraryRepository
    : IModelResourceRepository<SectionProfileId, SectionProfileFromLibrary> { }

internal class InMemorySectionProfileFromLibraryRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
)
    : InMemoryModelResourceRepository<SectionProfileId, SectionProfileFromLibrary>(
        inMemoryModelRepositoryStorage
    ),
        ISectionProfileFromLibraryRepository { }
