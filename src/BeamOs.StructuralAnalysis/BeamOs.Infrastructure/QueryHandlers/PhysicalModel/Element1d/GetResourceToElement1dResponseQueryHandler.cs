using BeamOs.Application.Common.Queries;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;
using BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Element1d.Mappers;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Element1d;

internal sealed class GetResourceToElement1dResponseQueryHandler(
    BeamOsStructuralDbContext dbContext
) : IQueryHandler<GetResourceByIdQuery, Element1DResponse>
{
    public async Task<Element1DResponse?> ExecuteAsync(
        GetResourceByIdQuery query,
        CancellationToken ct = default
    )
    {
        Element1DId element1DId = new(query.Id);
        var element1d = await dbContext
            .Element1Ds
            .AsNoTracking()
            .Where(m => m.Id == element1DId)
            .FirstAsync(cancellationToken: ct);

        var unitSettings = await dbContext
            .Models
            .Where(m => m.Id == element1d.ModelId)
            .Select(m => m.Settings.UnitSettings)
            .FirstAsync(cancellationToken: ct);

        var element1dToResponseMapper = Element1dToResponseMapper.Create(unitSettings);
        return element1dToResponseMapper.Map(element1d);
    }
}
