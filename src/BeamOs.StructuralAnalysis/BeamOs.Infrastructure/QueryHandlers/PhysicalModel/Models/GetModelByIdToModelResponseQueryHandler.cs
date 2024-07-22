using BeamOs.Api.PhysicalModel.Models.Mappers;
using BeamOs.Application.Common.Queries;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Models;

internal sealed class GetModelByIdToModelResponseQueryHandler(
    BeamOsStructuralReadModelDbContext dbContext,
    ModelReadModelToModelResponseMapper modelReadModelToModelResponseMapper
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

        if (query.Properties is not null)
        {
            foreach (string propertyName in query.Properties)
            {
                queryable = queryable.Include(propertyName);
            }
        }
        else
        {
            // todo : more generic
            queryable = queryable
                .Include(m => m.Nodes)
                .Include(m => m.Element1ds)
                .Include(m => m.Materials)
                .Include(m => m.SectionProfiles)
                .Include(m => m.PointLoads)
                .Include(m => m.MomentLoads);
        }

        ModelReadModel? model = await queryable.FirstOrDefaultAsync(cancellationToken: ct);

        return model is not null ? modelReadModelToModelResponseMapper.Map(model) : null;
    }
}
