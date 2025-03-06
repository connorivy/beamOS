using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.CsSdk.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Infrastructure.AnalyticalResults.ResultSets;

public class GetResultSetQueryHandler(StructuralAnalysisDbContext dbContext)
    : IQueryHandler<IModelEntity, ResultSetResponse>
{
    public async Task<Result<ResultSetResponse>> ExecuteAsync(
        IModelEntity query,
        CancellationToken ct = default
    )
    {
        var elementAndModelUnits = await dbContext
            .ResultSets
            .Where(e => e.ModelId.Equals(query.ModelId) && e.Id.Equals(query.Id))
            .Include(e => e.NodeResults)
            .Include(e => e.Element1dResults)
            .AsSplitQuery()
            .Select(el => new { el, el.Model.Settings.UnitSettings })
            .FirstOrDefaultAsync(cancellationToken: ct);

        if (elementAndModelUnits is null)
        {
            return BeamOsError.NotFound();
        }

        var mapper = ResultSetToResponseMapper.Create(elementAndModelUnits.UnitSettings);
        return mapper.Map(elementAndModelUnits.el);
    }
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappersJustEnums))]
internal partial class ResultSetToResponseMapper
    : AbstractMapperProvidedUnits<ResultSet, ResultSetResponse>
{
    [Obsolete()]
    public ResultSetToResponseMapper()
        : base(null) { }

    private ResultSetToResponseMapper(UnitSettings unitSettings)
        : base(unitSettings) { }

    public static ResultSetToResponseMapper Create(UnitSettings unitSettings)
    {
        return new(unitSettings);
    }

    public ResultSetResponse Map(ResultSet source) => this.ToResponse(source);

    //[MapProperty(nameof(NodeResult.Id), nameof(NodeResultResponse.NodeId))]
    private partial ResultSetResponse ToResponse(ResultSet source);
}
