using BeamOS.Common.Application.Interfaces;
using BeamOs.Infrastructure.PhysicalModel;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.PhysicalModel.PointLoads;

public class PointLoadDbContextRepository(PhysicalModelDbContext dbContext)
    : IRepository<PointLoadId, PointLoad>
{
    public async Task Add(PointLoad aggregate)
    {
        _ = await dbContext.PointLoads.AddAsync(aggregate);
        _ = await dbContext.SaveChangesAsync();
    }

    public async Task<PointLoad?> GetById(PointLoadId id)
    {
        return await dbContext.PointLoads.FirstAsync(el => el.Id == id);
    }
}
