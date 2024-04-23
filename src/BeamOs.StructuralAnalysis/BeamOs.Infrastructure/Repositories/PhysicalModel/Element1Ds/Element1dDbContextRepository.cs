using BeamOs.Application.Common.Interfaces;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;
using BeamOs.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.Repositories.PhysicalModel.Element1Ds;

internal class Element1dDbContextRepository(
    BeamOsStructuralDbContext dbContext,
    BeamOsStructuralReadModelDbContext readModelDbContext
) : IRepository<Element1DId, Element1D>
{
    public async Task Add(Element1D aggregate)
    {
        _ = await dbContext.Element1Ds.AddAsync(aggregate);
        _ = await dbContext.SaveChangesAsync();
    }

    public async Task<Element1D?> GetById(Element1DId id)
    {
        var x = readModelDbContext.Element1Ds.Where(el => el.Id == id.Value).Take(1);

        //var y = await x.Select(el => el.StartNode).Concat(x.Select(el => el.EndNode)).ToListAsync();

        return await dbContext.Element1Ds.FirstOrDefaultAsync(el => el.Id == id);
    }
}
