using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;

public interface ISectionProfileRepository : IRepository<SectionProfileId, SectionProfile>
{
    //public Task<List<SectionProfile>> GetSectionProfilesInModel(
    //    ModelId modelId,
    //    CancellationToken ct = default
    //);
}
