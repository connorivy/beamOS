using BeamOs.Application.Common.Interfaces;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.Repositories.PhysicalModel.Models;

public class ModelDbContextRepository(BeamOsStructuralDbContext dbContext)
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
