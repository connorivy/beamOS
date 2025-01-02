using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.CsSdk.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.NodeResult;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Element1ds;
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Infrastructure.AnalyticalResults.NodeResults;

public class GetNodeResultQueryHandler(StructuralAnalysisDbContext dbContext)
    : IQueryHandler<GetAnalyticalResultResourceQuery, NodeResultResponse>
{
    public async Task<Result<NodeResultResponse>> ExecuteAsync(
        GetAnalyticalResultResourceQuery query,
        CancellationToken ct = default
    )
    {
        var elementAndModelUnits = await dbContext
            .NodeResults
            .Where(
                e =>
                    e.ModelId.Equals(query.ModelId)
                    && e.ResultSetId == query.ResultSetId
                    && e.Id.Equals(query.Id)
            )
            .Select(el => new { el, el.Model.Settings.UnitSettings })
            .FirstOrDefaultAsync(cancellationToken: ct);

        if (elementAndModelUnits is null)
        {
            return BeamOsError.NotFound();
        }

        var mapper = NodeResultToResponseMapper.Create(elementAndModelUnits.UnitSettings);
        return mapper.Map(elementAndModelUnits.el);
    }
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappersJustEnums))]
internal partial class NodeResultToResponseMapper
    : AbstractMapperProvidedUnits<NodeResult, NodeResultResponse>
{
    [Obsolete()]
    public NodeResultToResponseMapper()
        : base(null) { }

    private NodeResultToResponseMapper(UnitSettings unitSettings)
        : base(unitSettings) { }

    public static NodeResultToResponseMapper Create(UnitSettings unitSettings)
    {
        return new(unitSettings);
    }

    public NodeResultResponse Map(NodeResult source) => this.ToResponse(source);

    [MapProperty(nameof(NodeResult.Id), nameof(NodeResultResponse.NodeId))]
    private partial NodeResultResponse ToResponse(NodeResult source);
}
