using BeamOs.Application.Common.Interfaces;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.PhysicalModel.PointLoads;

public class PointLoadDbContextRepository(BeamOsStructuralDbContext dbContext)
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
