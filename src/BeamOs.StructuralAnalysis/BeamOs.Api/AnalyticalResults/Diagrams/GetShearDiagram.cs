using BeamOs.Api.AnalyticalResults.Diagrams.Mappers;
using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Application.Common.Queries;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.AnalyticalResults.Diagrams;
using BeamOs.Contracts.Common;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.Diagrams.MomentDiagramAggregate;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;
using FastEndpoints;

namespace BeamOs.Api.PhysicalModel.Element1ds.Endpoints;

public class GetShearDiagram(
    BeamOsFastEndpointOptions options,
    IQueryHandler<GetResourceByIdQuery, ShearForceDiagram> getShearDiagramQueryHandler,
    ShearDiagramDataToResponse responseMapper
) : BeamOsFastEndpoint<IdRequest, ShearDiagramResponse?>(options)
{
    public override Http EndpointType => Http.GET;

    public override string Route => "element1Ds/{id}/diagrams/shear/";

    public override async Task<ShearDiagramResponse?> ExecuteAsync(
        IdRequest req,
        CancellationToken ct
    )
    {
        GetResourceByIdQuery query = new(Guid.Parse(req.Id));

        ShearForceDiagram? data = await getShearDiagramQueryHandler.ExecuteAsync(query, ct);

        if (data is null)
        {
            return null;
        }

        //var modelData = await getModelQueryHandler.ExecuteAsync(new(data.ModelId, []), ct);
        //data.UseUnitSettings(modelData.Settings.UnitSettings);

        return responseMapper.Map(data);
    }
}

public class GetMomentDiagram(
    BeamOsFastEndpointOptions options,
    IQueryHandler<GetResourceByIdQuery, MomentDiagram> getShearDiagramQueryHandler,
    IQueryHandler<GetResourceByIdQuery, UnitSettings> unitSettingsQueryHandler
) : BeamOsFastEndpoint<IdRequest, MomentDiagramResponse?>(options)
{
    public override Http EndpointType => Http.GET;

    public override string Route => "element1Ds/{id}/diagrams/moment/";

    public override async Task<MomentDiagramResponse?> ExecuteAsync(
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
