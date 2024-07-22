using BeamOs.Application.Common.Queries;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Element1d.Mappers;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Element1d;

internal sealed class GetResourceToElement1dResponseQueryHandler(
    BeamOsStructuralReadModelDbContext dbContext,
    Element1dReadModelToResponseMapper element1dReadModelToResponseMapper
) : IQueryHandler<GetResourceByIdQuery, Element1DResponse>
{
    public async Task<Element1DResponse?> ExecuteAsync(
        GetResourceByIdQuery query,
        CancellationToken ct = default
    )
    {
        return await dbContext
            .Element1Ds
            .Where(m => m.Id == query.Id)
            .Take(1)
            .Select(el => element1dReadModelToResponseMapper.Map(el))
            .FirstOrDefaultAsync(cancellationToken: ct);
    }
}
