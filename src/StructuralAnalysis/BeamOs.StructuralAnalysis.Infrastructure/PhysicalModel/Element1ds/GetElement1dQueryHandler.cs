using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.CsSdk.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Element1ds;

public class GetElement1dQueryHandler(StructuralAnalysisDbContext dbContext)
    : IQueryHandler<IModelEntity, Element1dResponse>
{
    public async Task<Result<Element1dResponse>> ExecuteAsync(
        IModelEntity query,
        CancellationToken ct = default
    )
    {
        var elementAndModelUnits = await dbContext
            .Element1ds
            .Where(e => e.ModelId.Equals(query.ModelId) && e.Id.Equals(query.Id))
            .Select(el => new { el, el.Model.Settings.UnitSettings })
            .FirstOrDefaultAsync(cancellationToken: ct);

        if (elementAndModelUnits is null)
        {
            return BeamOsError.NotFound();
        }

        var mapper = Element1dToResponseMapper.Create(elementAndModelUnits.UnitSettings);
        return mapper.Map(elementAndModelUnits.el);
    }
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappersJustEnums))]
internal partial class Element1dToResponseMapper
    : AbstractMapperProvidedUnits<Element1d, Element1dResponse>
{
    [Obsolete()]
    public Element1dToResponseMapper()
        : base(null) { }

    private Element1dToResponseMapper(UnitSettings unitSettings)
        : base(unitSettings) { }

    public static Element1dToResponseMapper Create(UnitSettings unitSettings)
    {
        return new(unitSettings);
    }

    public Element1dResponse Map(Element1d source) => this.ToResponse(source);

    private partial Element1dResponse ToResponse(Element1d source);
}
