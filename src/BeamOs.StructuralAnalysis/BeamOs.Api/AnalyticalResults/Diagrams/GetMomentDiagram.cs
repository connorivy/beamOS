using BeamOs.Api.AnalyticalResults.Diagrams.Mappers;
using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Application.Common.Queries;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.AnalyticalResults.Diagrams;
using BeamOs.Contracts.Common;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.Diagrams.MomentDiagramAggregate;
using FastEndpoints;

namespace BeamOs.Api.PhysicalModel.Element1ds.Endpoints;

public class GetMomentDiagram(
    BeamOsFastEndpointOptions options,
    IQueryHandler<GetResourceByIdQuery, MomentDiagram> getShearDiagramQueryHandler,
    IQueryHandler<GetResourceByIdQuery, UnitSettings> unitSettingsQueryHandler
) : BeamOsFastEndpoint<IdRequest, MomentDiagramResponse?>(options)
{
    public override Http EndpointType => Http.GET;

    public override string Route => "element1Ds/{id}/diagrams/moment/";

    public override async Task<MomentDiagramResponse?> ExecuteRequestAsync(
        IdRequest req,
        CancellationToken ct
    )
    {
        GetResourceByIdQuery query = new(Guid.Parse(req.Id));

        MomentDiagram? data = await getShearDiagramQueryHandler.ExecuteAsync(query, ct);

        if (data is null)
        {
            return null;
        }

        var unitSettings = await unitSettingsQueryHandler.ExecuteAsync(new(data.ModelId.Id), ct);
        var responseMapper = MomentDiagramDataToResponse.Create(unitSettings);

        return responseMapper.Map(data);
    }
}
