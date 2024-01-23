using BeamOS.Common.Application.Interfaces;
using BeamOS.PhysicalModel.Domain.MomentLoadAggregate;
using BeamOS.PhysicalModel.Domain.MomentLoadAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOS.PhysicalModel.Infrastructure.MomentLoads;

public class MomentLoadDbContextRepository(PhysicalModelDbContext dbContext)
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
