using BeamOS.Common.Application.Interfaces;
using BeamOS.PhysicalModel.Domain.MaterialAggregate;
using BeamOS.PhysicalModel.Domain.MaterialAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOS.PhysicalModel.Infrastructure.Materials;

public class MaterialDbContextRepository(PhysicalModelDbContext dbContext)
    : IRepository<MaterialId, Material>
{
    public async Task Add(Material aggregate)
    {
        _ = await dbContext.Materials.AddAsync(aggregate);
        _ = await dbContext.SaveChangesAsync();
    }

    public async Task<Material?> GetById(MaterialId id)
    {
        return await dbContext.Materials.FirstOrDefaultAsync(el => el.Id == id);
    }
}
