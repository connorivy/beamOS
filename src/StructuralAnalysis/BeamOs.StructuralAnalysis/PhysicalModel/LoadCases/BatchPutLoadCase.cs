using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.LoadCases;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "load-cases")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class BatchPutLoadCase(BatchPutLoadCaseCommandHandler putLoadCaseCommandHandler)
    : BeamOsModelResourceBaseEndpoint<BatchPutLoadCaseCommand, LoadCaseContract[], BatchResponse>
{
    public override async Task<Result<BatchResponse>> ExecuteRequestAsync(
        BatchPutLoadCaseCommand req,
        CancellationToken ct = default
    ) => await putLoadCaseCommandHandler.ExecuteAsync(req, ct);
}
