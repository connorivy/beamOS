using BeamOs.Application.Common.Interfaces;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.PhysicalModel.Element1Ds;

public class Element1dDbContextRepository(PhysicalModelDbContext dbContext)
    : IRepository<Element1DId, Element1D>
{
    public async Task Add(Element1D aggregate)
    {
        _ = await dbContext.Element1Ds.AddAsync(aggregate);
        _ = await dbContext.SaveChangesAsync();
    }

    public async Task<Element1D?> GetById(Element1DId id)
    {
        return await dbContext.Element1Ds.FirstOrDefaultAsync(el => el.Id == id);
    }
}
