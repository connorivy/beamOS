using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.LoadCases;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "load-cases")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class BatchPutLoadCase(BatchPutLoadCaseCommandHandler putLoadCaseCommandHandler)
    : BeamOsModelResourceBaseEndpoint<BatchPutLoadCaseCommand, LoadCaseResponse[], BatchResponse>
{
    public override async Task<Result<BatchResponse>> ExecuteRequestAsync(
        BatchPutLoadCaseCommand req,
        CancellationToken ct = default
    ) => await putLoadCaseCommandHandler.ExecuteAsync(req, ct);
}

public sealed class BatchPutLoadCaseCommandHandler(
    ILoadCaseRepository repository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : BatchPutCommandHandler<LoadCaseId, LoadCase, BatchPutLoadCaseCommand, LoadCaseResponse>(
        repository,
        unitOfWork
    )
{
    protected override LoadCase ToDomainObject(ModelId modelId, LoadCaseResponse putRequest) =>
        new PutLoadCaseCommand(modelId, putRequest).ToDomainObject();
}

public readonly struct BatchPutLoadCaseCommand : IModelResourceRequest<LoadCaseResponse[]>
{
    public Guid ModelId { get; init; }
    public LoadCaseResponse[] Body { get; init; }
}
