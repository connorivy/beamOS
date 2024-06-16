using BeamOs.Application.PhysicalModel.SectionProfiles;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.Repositories.PhysicalModel.SectionProfiles;

internal class SectionProfileDbContextRepository(BeamOsStructuralDbContext dbContext)
    : RepositoryBase<SectionProfileId, SectionProfile>(dbContext),
        ISectionProfileRepository
{
    public async Task<List<SectionProfile>> GetSectionProfilesInModel(
        ModelId modelId,
        CancellationToken ct = default
    )
    {
        return await this.DbContext
            .SectionProfiles
            .Where(s => s.ModelId == modelId)
            .ToListAsync(cancellationToken: ct);
    }
}
