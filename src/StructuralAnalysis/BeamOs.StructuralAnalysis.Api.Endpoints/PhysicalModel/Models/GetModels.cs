using BeamOs.Common.Api;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Models;

[BeamOsRoute("models/")]
[BeamOsEndpointType(Http.Get)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Authenticated)]
public class GetModels(IQueryHandler<EmptyRequest, List<ModelInfoResponse>> getModelsCommandHandler)
    : BeamOsEmptyRequestBaseEndpoint<List<ModelInfoResponse>>
{
    public override async Task<Result<List<ModelInfoResponse>>> ExecuteRequestAsync(
        EmptyRequest req,
        CancellationToken ct = default
    ) => await getModelsCommandHandler.ExecuteAsync(req, ct);
}
