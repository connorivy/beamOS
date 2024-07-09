using BeamOs.Application.Common.Queries;
using BeamOs.Application.PhysicalModel.Element1dAggregate.Interfaces;
using BeamOs.Common.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Element1d;

internal class GetSingleElement1d
    : IQueryHandler<GetResourceByIdWithPropertiesQuery, IElement1dData>
{
    private readonly BeamOsStructuralReadModelDbContext dbContext;

    public GetSingleElement1d(BeamOsStructuralReadModelDbContext dbContext) =>
        this.dbContext = dbContext;

    public async Task<IElement1dData?> ExecuteAsync(
        GetResourceByIdWithPropertiesQuery query,
        CancellationToken ct = default
    )
    {
        var element1dQuery = this.dbContext.Element1Ds.Where(el => el.Id == query.Id);

        if (query.Properties is not null)
        {
            foreach (string propertyName in query.Properties)
            {
                element1dQuery = element1dQuery.Include(propertyName);
            }
        }
        else
        {
            // todo : more generic
            element1dQuery = element1dQuery
                .Include(el => el.StartNode)
                .Include(el => el.EndNode)
                .Include(el => el.Material)
                .Include(el => el.SectionProfile);
        }

        return await element1dQuery.FirstOrDefaultAsync(ct);
    }
}
