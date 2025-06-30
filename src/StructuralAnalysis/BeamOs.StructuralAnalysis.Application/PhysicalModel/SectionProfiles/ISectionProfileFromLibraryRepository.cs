using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;

public interface ISectionProfileFromLibraryRepository
    : IModelResourceRepository<SectionProfileId, SectionProfileFromLibrary> { }

public class InMemorySectionProfileFromLibraryRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
)
    : InMemoryModelResourceRepository<SectionProfileId, SectionProfileFromLibrary>(
        inMemoryModelRepositoryStorage
    ),
        ISectionProfileFromLibraryRepository { }
