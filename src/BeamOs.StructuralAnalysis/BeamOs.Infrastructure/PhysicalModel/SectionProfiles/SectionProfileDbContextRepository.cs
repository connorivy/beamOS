using BeamOs.Application.Common.Interfaces;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.PhysicalModel.SectionProfiles;

public class SectionProfileDbContextRepository(BeamOsStructuralDbContext dbContext)
    : IRepository<SectionProfileId, SectionProfile>
{
    public async Task Add(SectionProfile aggregate)
    {
        _ = await dbContext.SectionProfiles.AddAsync(aggregate);
        _ = await dbContext.SaveChangesAsync();
    }

    public async Task<SectionProfile?> GetById(SectionProfileId id)
    {
        return await dbContext.SectionProfiles.FirstOrDefaultAsync(el => el.Id == id);
    }
}
