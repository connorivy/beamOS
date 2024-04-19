using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.PhysicalModel.Element1dAggregate.Queries;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Element1d;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Element1d;

internal class GetSingleElement1d : IQueryHandler<GetElement1dByIdQuery, Element1DResponse>
{
    private readonly BeamOsStructuralReadModelDbContext dbContext;

    public GetSingleElement1d(BeamOsStructuralReadModelDbContext dbContext) =>
        this.dbContext = dbContext;

    public async Task<Element1DResponse?> ExecuteAsync(
        GetElement1dByIdQuery query,
        CancellationToken ct = default
    )
    {
        return await this.dbContext
            .Element1Ds
            .Where(el => el.Id == query.Id)
            .Take(1)
            .Select(
                el =>
                    new Element1DResponse(
                        el.Id.ToString(),
                        el.ModelId.ToString(),
                        el.StartNodeId.ToString(),
                        el.EndNodeId.ToString(),
                        el.MaterialId.ToString(),
                        el.SectionProfileId.ToString(),
                        new UnitValueDto(
                            el.SectionProfileRotation.Value,
                            el.SectionProfileRotation.Unit.ToString()
                        ),
                        null,
                        null,
                        null,
                        null
                    )
            )
            .FirstOrDefaultAsync(ct);
    }
}
