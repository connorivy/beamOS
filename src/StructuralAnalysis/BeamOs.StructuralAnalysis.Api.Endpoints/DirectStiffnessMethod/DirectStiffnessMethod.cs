using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Api.Endpoints.OpenSees;
using BeamOs.StructuralAnalysis.Application.DirectStiffnessMethod;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.DirectStiffnessMethod;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "analyze/dsm")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class DirectStiffnessMethod(RunDirectStiffnessMethodCommandHandler dsmCommandHandler)
    : BeamOsModelIdRequestBaseEndpoint<bool>
{
    public override async Task<Result<bool>> ExecuteRequestAsync(
        ModelIdRequest req,
        CancellationToken ct = default
    ) => await dsmCommandHandler.ExecuteAsync(req.ModelId, ct);
}
