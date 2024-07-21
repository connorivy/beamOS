using BeamOs.Application.Common.Queries;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Models;

internal class GetUnitSettingsQueryHandler(BeamOsStructuralReadModelDbContext dbContext)
    : IQueryHandler<GetResourceByIdQuery, UnitSettings>
{
    public async Task<UnitSettings?> ExecuteAsync(
        GetResourceByIdQuery query,
        CancellationToken ct = default
    )
    {
        IQueryable<ModelReadModel> queryable = dbContext
            .Models
            .Where(m => m.Id == query.Id)
            .Take(1);

        return (await queryable.FirstOrDefaultAsync(ct))?.Settings.UnitSettings;
    }
}
