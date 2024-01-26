using BeamOS.Common.Application.Interfaces;
using BeamOS.PhysicalModel.Domain.ModelAggregate;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOS.PhysicalModel.Infrastructure.Models;

public class ModelDbContextRepository(PhysicalModelDbContext dbContext)
    : IRepository<ModelId, Model>
{
    public async Task Add(Model aggregate)
    {
        _ = await dbContext.Models.AddAsync(aggregate);
        _ = await dbContext.SaveChangesAsync();
    }

    public async Task<Model?> GetById(ModelId id)
    {
        return await dbContext.Models.FirstAsync(el => el.Id == id);
    }
}
