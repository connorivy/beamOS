using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate.ValueObjects;

namespace BeamOs.Application.PhysicalModel.SectionProfiles;

public interface ISectionProfileRepository : IRepository<SectionProfileId, SectionProfile>
{
    public Task<List<SectionProfile>> GetSectionProfilesInModel(
        ModelId modelId,
        CancellationToken ct = default
    );
}
