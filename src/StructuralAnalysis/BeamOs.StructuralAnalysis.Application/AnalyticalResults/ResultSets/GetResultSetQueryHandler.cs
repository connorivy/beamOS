using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.CsSdk.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.ResultSets;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Infrastructure.AnalyticalResults.ResultSets;

public class GetResultSetQueryHandler(IResultSetRepository resultSetRepository)
    : IQueryHandler<IModelEntity, ResultSetResponse>
{
    public async Task<Result<ResultSetResponse>> ExecuteAsync(
        IModelEntity query,
        CancellationToken ct = default
    )
    {
        var elementAndModelUnits = await resultSetRepository.GetSingleWithModelSettings(
            query.ModelId,
            query.Id,
            ct
        );

        if (elementAndModelUnits is null)
        {
            return BeamOsError.NotFound();
        }

        var mapper = ResultSetToResponseMapper.Create(
            elementAndModelUnits.Value.ModelSettings.UnitSettings
        );
        return mapper.Map(elementAndModelUnits.Value.Entity);
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
