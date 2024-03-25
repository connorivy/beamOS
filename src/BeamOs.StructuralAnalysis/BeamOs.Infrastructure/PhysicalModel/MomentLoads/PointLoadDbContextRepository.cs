using BeamOs.Application.Common.Interfaces;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.PhysicalModel.MomentLoads;

public class MomentLoadDbContextRepository(BeamOsStructuralDbContext dbContext)
    : IRepository<MomentLoadId, MomentLoad>
{
    public async Task Add(MomentLoad aggregate)
    {
        _ = await dbContext.MomentLoads.AddAsync(aggregate);
        _ = await dbContext.SaveChangesAsync();
    }

    public async Task<MomentLoad?> GetById(MomentLoadId id)
    {
        return await dbContext.MomentLoads.FirstAsync(el => el.Id == id);
    }
}
