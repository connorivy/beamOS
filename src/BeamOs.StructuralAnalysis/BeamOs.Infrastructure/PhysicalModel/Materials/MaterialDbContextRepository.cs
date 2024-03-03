using BeamOs.Application.Common.Interfaces;
using BeamOs.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.Domain.PhysicalModel.MaterialAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.PhysicalModel.Materials;

public class MaterialDbContextRepository(BeamOsStructuralDbContext dbContext)
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
