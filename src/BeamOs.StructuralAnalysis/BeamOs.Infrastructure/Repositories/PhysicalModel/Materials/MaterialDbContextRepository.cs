using BeamOs.Application.PhysicalModel.Materials;
using BeamOs.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.Domain.PhysicalModel.MaterialAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.Repositories.PhysicalModel.Materials;

internal class MaterialDbContextRepository(BeamOsStructuralDbContext dbContext)
    : RepositoryBase<MaterialId, Material>(dbContext),
        IMaterialRepository
{
    public async Task<List<Material>> GetMaterialsInModel(
        ModelId modelId,
        CancellationToken ct = default
    )
    {
        return await this.DbContext
            .Materials
            .Where(m => m.ModelId == modelId)
            .ToListAsync(cancellationToken: ct);
    }
}
