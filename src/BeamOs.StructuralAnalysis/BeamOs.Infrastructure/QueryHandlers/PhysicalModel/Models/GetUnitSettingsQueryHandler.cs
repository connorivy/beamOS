using BeamOs.Application.Common.Queries;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Models;

internal class GetUnitSettingsQueryHandler(BeamOsStructuralDbContext dbContext)
    : IQueryHandler<GetResourceByIdQuery, UnitSettings>
{
    public async Task<UnitSettings?> ExecuteAsync(
        GetResourceByIdQuery query,
        CancellationToken ct = default
    )
    {
        ModelId modelId = new(query.Id);
        IQueryable<Model> queryable = dbContext.Models.Where(m => m.Id == modelId).Take(1);

        return (await queryable.FirstOrDefaultAsync(ct))?.Settings.UnitSettings;
    }
}
