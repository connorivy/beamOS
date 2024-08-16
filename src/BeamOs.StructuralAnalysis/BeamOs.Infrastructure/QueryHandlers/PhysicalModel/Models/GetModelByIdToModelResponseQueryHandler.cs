using BeamOs.Api.PhysicalModel.Models.Mappers;
using BeamOs.Application.Common.Queries;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Models;

internal sealed class GetModelByIdToModelResponseQueryHandler(BeamOsStructuralDbContext dbContext)
    : IQueryHandler<GetResourceByIdWithPropertiesQuery, ModelResponse>
{
    public async Task<ModelResponse?> ExecuteAsync(
        GetResourceByIdWithPropertiesQuery query,
        CancellationToken ct = default
    )
    {
        ModelId modelId = new(query.Id);
        IQueryable<Model> queryable = dbContext
            .Models
            .AsSplitQuery()
            .Where(m => m.Id == modelId)
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

        Model? model = await queryable.FirstOrDefaultAsync(cancellationToken: ct);
        var modelToModelResponseMapper = ModelToModelResponseMapper.Create(
            model.Settings.UnitSettings
        );

        return model is not null ? modelToModelResponseMapper.Map(model) : null;
    }
}
