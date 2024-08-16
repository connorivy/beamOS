using BeamOs.Api.AnalyticalResults.Diagrams.Mappers;
using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Application.Common.Queries;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.AnalyticalResults.Diagrams;
using BeamOs.Contracts.Common;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;
using FastEndpoints;

namespace BeamOs.Api.PhysicalModel.Element1ds.Endpoints;

public class GetShearDiagrams(
    BeamOsFastEndpointOptions options,
    IQueryHandler<GetResourceByIdQuery, ShearForceDiagram[]> getShearDiagramQueryHandler,
    ShearDiagramDataToResponse responseMapper
) : BeamOsFastEndpoint<ModelIdRequest, ShearDiagramResponse[]>(options)
{
    public override Http EndpointType => Http.GET;

    public override string Route => "model/{modelId}/diagrams/shear";

    public override async Task<ShearDiagramResponse[]> ExecuteRequestAsync(
        ModelIdRequest req,
        CancellationToken ct
    )
    {
        GetResourceByIdQuery query = new(Guid.Parse(req.ModelId));

        ShearForceDiagram[] data = await getShearDiagramQueryHandler.ExecuteAsync(query, ct);

        if (data.Length == 0)
        {
            return [];
        }

        //var modelData = await getModelQueryHandler.ExecuteAsync(new(data.ModelId, []), ct);
        //data.UseUnitSettings(modelData.Settings.UnitSettings);

        return data.Select(responseMapper.Map).ToArray();
    }
}
