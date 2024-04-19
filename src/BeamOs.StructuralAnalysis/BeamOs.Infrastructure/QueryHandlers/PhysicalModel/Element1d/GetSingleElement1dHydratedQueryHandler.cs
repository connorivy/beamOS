using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.PhysicalModel.Element1dAggregate.Queries;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Element1d.Mappers;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Element1d;

internal class GetSingleElement1dHydratedQueryHandler(
    BeamOsStructuralReadModelDbContext dbContext,
    Element1dReadModelToResponseMapper hydratedResponseMapper
) : IQueryHandler<GetElement1dHydratedByIdQuery, Element1dResponseHydrated>
{
    public async Task<Element1dResponseHydrated?> ExecuteAsync(
        GetElement1dHydratedByIdQuery query,
        CancellationToken ct = default
    )
    {
        return await dbContext
            .Element1Ds
            .Where(el => el.Id == query.Id)
            .Take(1)
            .Include(el => el.StartNode)
            .Include(el => el.EndNode)
            .Include(el => el.Material)
            .Include(el => el.SectionProfile)
            .Select(el => hydratedResponseMapper.Map(el))
            .FirstOrDefaultAsync(ct);
    }
}
