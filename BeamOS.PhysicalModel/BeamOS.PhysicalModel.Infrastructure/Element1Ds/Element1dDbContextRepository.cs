using BeamOS.Common.Application.Interfaces;
using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using BeamOS.PhysicalModel.Domain.Element1DAggregate.ValueObjects;

namespace BeamOS.PhysicalModel.Infrastructure.Element1Ds;
public class Element1dDbContextRepository(PhysicalModelDbContext dbContext) : IRepository<Element1DId, Element1D>
{
    public async Task Add(Element1D aggregate)
    {
        _ = await dbContext.Element1Ds.AddAsync(aggregate);
        _ = await dbContext.SaveChangesAsync();
    }
    public Task<Element1D?> GetById(Element1DId id)
    {
        return Task.FromResult(dbContext.Element1Ds
            .Where(el => el.Id == id)
            .FirstOrDefault());
    }
}
