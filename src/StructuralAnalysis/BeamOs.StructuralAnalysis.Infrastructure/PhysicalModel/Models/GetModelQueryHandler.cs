using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.CsSdk.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Element1ds;
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Models;

public class GetModelQueryHandler(StructuralAnalysisDbContext dbContext)
    : IQueryHandler<Guid, ModelResponseHydrated>
{
    public async Task<Result<ModelResponseHydrated>> ExecuteAsync(
        Guid query,
        CancellationToken ct = default
    )
    {
        var model = await dbContext
            .Models
            .AsNoTracking()
            .Where(e => e.Id.Equals(query))
            .Include(m => m.Nodes)
            .Include(m => m.PointLoads)
            .Include(m => m.MomentLoads)
            .Include(m => m.Element1ds)
            .Include(el => el.SectionProfiles)
            .Include(el => el.Materials)
            .FirstOrDefaultAsync(cancellationToken: ct);

        if (model is null)
        {
            return BeamOsError.NotFound();
        }

        var mapper = ModelToResponseMapper.Create(model.Settings.UnitSettings);
        return mapper.Map(model);
    }
}

[Mapper]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
[UseStaticMapper(typeof(UnitsNetMappersJustEnums))]
internal partial class ModelToResponseMapper : AbstractMapperProvidedUnits<Model, ModelResponse>
{
    [Obsolete()]
    public ModelToResponseMapper()
        : base(null) { }

    private ModelToResponseMapper(UnitSettings unitSettings)
        : base(unitSettings) { }

    public static ModelToResponseMapper Create(UnitSettings unitSettings)
    {
        return new(unitSettings);
    }

    public ModelResponseHydrated Map(Model source) => this.ToResponse(source);

    private partial ModelResponseHydrated ToResponse(Model source);
}
