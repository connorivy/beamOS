using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.CsSdk.Mappers;

namespace BeamOs.SpeckleConnector;

[BeamOsRoute("speckle-receive")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Authenticated)]
public class ConvertToBeamOs
    : BeamOsFromBodyBaseEndpoint<SpeckleReceiveParameters, BeamOsModelBuilderDto>
{
    public override async Task<Result<BeamOsModelBuilderDto>> ExecuteRequestAsync(
        SpeckleReceiveParameters req,
        CancellationToken ct = default
    )
    {
        var modelBuilder = await new BeamOsModelBuilderSpeckleFactory(
            "doesn't matter",
            new(UnitSettingsContract.kN_M),
            "doesn't matter",
            "doesn't matter"
        ).Build(req.ApiToken, req.ProjectId, req.ObjectId, req.ServerUrl, ct);

        return modelBuilder.ToDto();
    }
}
