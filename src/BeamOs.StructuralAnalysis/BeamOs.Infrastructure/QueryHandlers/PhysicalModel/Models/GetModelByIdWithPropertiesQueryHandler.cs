using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.Common.Queries;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.Common.Extensions;
using BeamOs.Infrastructure.Data.Models;
using BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Models.Mappers;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Models;

internal sealed class GetModelByIdWithPropertiesQueryHandler(
    BeamOsStructuralReadModelDbContext dbContext,
    ModelReadModelToModelResponseMapper modelReadModelMapper,
    IFlattenedModelSettingsToUnitSettingsDomainObjectMapper flattenedModelSettingsToDomainObjectMapper
) : IQueryHandler<GetResourceByIdWithPropertiesQuery, ModelResponse>
{
    public async Task<ModelResponse?> ExecuteAsync(
        GetResourceByIdWithPropertiesQuery query,
        CancellationToken ct = default
    )
    {
        IQueryable<ModelReadModel> queryable = dbContext
            .Models
            .Where(m => m.Id == query.Id)
            .Take(1);

        ModelReadModel? readModel = await queryable
            .Include(m => m.Nodes)
            .ThenInclude(n => n.PointLoads)
            .Include(m => m.Nodes)
            .ThenInclude(n => n.MomentLoads)
            .Include(m => m.Element1ds)
            .Include(m => m.Materials)
            .Include(m => m.SectionProfiles)
            .FirstOrDefaultAsync(ct);

        UnitSettings unitSettings = flattenedModelSettingsToDomainObjectMapper.Map(readModel);
        readModel.UseUnitSettings(unitSettings);

        return modelReadModelMapper.Map(readModel);
    }
}
