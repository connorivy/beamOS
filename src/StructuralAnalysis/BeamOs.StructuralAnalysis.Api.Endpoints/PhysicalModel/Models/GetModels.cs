using BeamOs.Common.Api;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Models;

[BeamOsRoute("models/")]
[BeamOsEndpointType(Http.Get)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Authenticated)]
public class GetModels(IQueryHandler<EmptyRequest, ICollection<ModelInfoResponse>> getModelsCommandHandler)
    : BeamOsEmptyRequestBaseEndpoint<ICollection<ModelInfoResponse>>
{
    public override async Task<Result<ICollection<ModelInfoResponse>>> ExecuteRequestAsync(
        EmptyRequest req,
        CancellationToken ct = default
    ) => await getModelsCommandHandler.ExecuteAsync(req, ct);
}
