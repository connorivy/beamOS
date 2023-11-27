using BeamOS.Common.Application.Interfaces;
using BeamOS.PhysicalModel.Domain.SectionProfileAggregate;
using BeamOS.PhysicalModel.Domain.SectionProfileAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOS.PhysicalModel.Infrastructure.SectionProfiles;

public class SectionProfileDbContextRepository(PhysicalModelDbContext dbContext) : IRepository<SectionProfileId, SectionProfile>
{
    public async Task Add(SectionProfile aggregate)
    {
        _ = await dbContext.SectionProfiles.AddAsync(aggregate);
        _ = await dbContext.SaveChangesAsync();
    }
    public async Task<SectionProfile?> GetById(SectionProfileId id)
    {
        return await dbContext.SectionProfiles
            .FirstOrDefaultAsync(el => el.Id == id);
    }
}
