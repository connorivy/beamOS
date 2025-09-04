using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.CsSdk.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.NodeResult;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.AnalyticalResults.NodeResults;

public class GetNodeResultQueryHandler(INodeResultRepository nodeResultRepository)
    : IQueryHandler<GetAnalyticalResultResourceQuery, NodeResultResponse>
{
    public async Task<Result<NodeResultResponse>> ExecuteAsync(
        GetAnalyticalResultResourceQuery query,
        CancellationToken ct = default
    )
    {
        var elementAndModelUnits = await nodeResultRepository.GetSingleWithModelSettings(
            query.ModelId,
            query.Id,
            ct
        );

        if (elementAndModelUnits is null)
        {
            return BeamOsError.NotFound(
                description: $"Could not find node result for node {query.Id}, result set {query.LoadCombinationId}, and model {query.ModelId}"
            );
        }

        var mapper = NodeResultToResponseMapper.Create(
            elementAndModelUnits.Value.ModelSettings.UnitSettings
        );
        return mapper.Map(elementAndModelUnits.Value.Entity);
    }
}

public class GetNodeResultsQueryHandler(INodeResultRepository nodeResultRepository)
    : IQueryHandler<GetAnalyticalResultQuery, IDictionary<int, NodeResultResponse>>
{
    public async Task<Result<IDictionary<int, NodeResultResponse>>> ExecuteAsync(
        GetAnalyticalResultQuery query,
        CancellationToken ct = default
    )
    {
        var elementAndModelUnits =
            await nodeResultRepository.GetAllFromLoadCombinationWithModelSettings(
                query.ModelId,
                query.LoadCombinationId,
                ct
            );

        if (elementAndModelUnits is null)
        {
            return BeamOsError.NotFound(
                description: $"Could not find node result for result set {query.LoadCombinationId}, and model {query.ModelId}"
            );
        }

        var mapper = NodeResultToResponseMapper.Create(
            elementAndModelUnits.Value.ModelSettings.UnitSettings
        );

        return elementAndModelUnits.Value.Entity.ToDictionary(x => x.Id.Id, x => mapper.Map(x));
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
