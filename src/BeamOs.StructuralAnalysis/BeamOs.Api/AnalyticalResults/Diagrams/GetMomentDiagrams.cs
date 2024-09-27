using BeamOs.Api.AnalyticalResults.Diagrams.Mappers;
using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Application.Common.Queries;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Common.Identity.Policies;
using BeamOs.Contracts.AnalyticalModel.Diagrams;
using BeamOs.Contracts.Common;
using BeamOs.Domain.AnalyticalModel.AnalyticalResultsAggregate.ValueObjects;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.Diagrams.MomentDiagramAggregate;
using FastEndpoints;

namespace BeamOs.Api.AnalyticalResults.Diagrams;

public class GetMomentDiagrams(
    BeamOsFastEndpointOptions options,
    IQueryHandler<GetResourceByIdQuery, MomentDiagram[]> getShearDiagramQueryHandler,
    //IQueryHandler<GetResourceByIdQuery, UnitSettings> unitSettingsQueryHandler,
    IQueryHandler<AnalyticalResultsId, UnitSettings> unitSettingsQueryHandler
) : BeamOsFastEndpoint<ModelIdRequest, MomentDiagramResponse[]>(options)
{
    public override Http EndpointType => Http.GET;

    public override string Route => "model/{modelId}/diagrams/moment";

    public override void ConfigureAuthentication()
    {
        this.AllowAnonymous();
        this.Policy(p => p.Requirements.Add(new RequireModelReadAccess()));
    }

    public override async Task<MomentDiagramResponse[]> ExecuteRequestAsync(
        ModelIdRequest req,
        CancellationToken ct
    )
    {
        GetResourceByIdQuery query = new(Guid.Parse(req.ModelId));

        MomentDiagram[] data = await getShearDiagramQueryHandler.ExecuteAsync(query, ct);

        if (data.Length == 0)
        {
            return [];
        }

        var unitSettings = await unitSettingsQueryHandler.ExecuteAsync(
            new(data[0].ModelResultId.Id),
            ct
        );
        var responseMapper = MomentDiagramDataToResponse.Create(unitSettings);

        return data.Select(responseMapper.Map).ToArray();
    }
}
