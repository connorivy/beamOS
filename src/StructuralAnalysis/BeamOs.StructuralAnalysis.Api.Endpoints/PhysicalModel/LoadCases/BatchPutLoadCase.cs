using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using LoadCase = BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases.LoadCase;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.LoadCases;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "load-cases")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class BatchPutLoadCase(BatchPutLoadCaseCommandHandler putLoadCaseCommandHandler)
    : BeamOsModelResourceBaseEndpoint<BatchPutLoadCaseCommand, LoadCase[], BatchResponse>
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
    : BatchPutCommandHandler<
        LoadCaseId,
        Domain.PhysicalModel.LoadCases.LoadCase,
        BatchPutLoadCaseCommand,
        LoadCase
    >(repository, unitOfWork)
{
    protected override Domain.PhysicalModel.LoadCases.LoadCase ToDomainObject(
        ModelId modelId,
        LoadCase putRequest
    ) => new PutLoadCaseCommand(modelId, putRequest).ToDomainObject();
}

public readonly struct BatchPutLoadCaseCommand : IModelResourceRequest<LoadCase[]>
{
    public Guid ModelId { get; init; }
    public LoadCase[] Body { get; init; }
}
