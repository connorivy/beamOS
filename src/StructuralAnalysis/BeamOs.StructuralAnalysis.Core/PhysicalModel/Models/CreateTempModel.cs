using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Models;

[BeamOsRoute("models/temp")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.None)]
internal class CreateTempModel(CreateTempModelCommandHandler createTempModelCommandHandler)
    : BeamOsFromBodyResultBaseEndpoint<CreateModelRequest, ModelResponse>
{
    public override async Task<Result<ModelResponse>> ExecuteRequestAsync(
        CreateModelRequest req,
        CancellationToken ct = default
    ) => await createTempModelCommandHandler.ExecuteAsync(req, ct);
}
