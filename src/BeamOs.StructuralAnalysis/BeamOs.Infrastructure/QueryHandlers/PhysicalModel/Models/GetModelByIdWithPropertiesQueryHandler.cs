using BeamOs.Application.Common.Queries;
using BeamOs.Application.PhysicalModel.Models.Interfaces;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Models;

internal sealed class GetModelByIdWithPropertiesQueryHandler(
    BeamOsStructuralReadModelDbContext dbContext
) : IQueryHandler<GetResourceByIdWithPropertiesQuery, IModelData>
{
    public async Task<IModelData?> ExecuteAsync(
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
                .ThenInclude(n => n.PointLoads)
                .Include(m => m.Nodes)
                .ThenInclude(n => n.MomentLoads)
                .Include(m => m.Element1ds)
                .Include(m => m.Materials)
                .Include(m => m.SectionProfiles);
        }

        return await queryable.FirstOrDefaultAsync(ct);
    }
}
