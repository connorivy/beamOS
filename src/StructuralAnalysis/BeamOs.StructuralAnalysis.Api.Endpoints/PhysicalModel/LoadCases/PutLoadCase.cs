using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.LoadCases;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "load-cases/{id}")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class PutLoadCase(PutLoadCaseCommandHandler putLoadCaseCommandHandler)
    : BeamOsModelResourceWithIntIdBaseEndpoint<PutLoadCaseCommand, LoadCaseData, LoadCase>
{
    public override async Task<Result<LoadCase>> ExecuteRequestAsync(
        PutLoadCaseCommand req,
        CancellationToken ct = default
    ) => await putLoadCaseCommandHandler.ExecuteAsync(req, ct);
}

public sealed class PutLoadCaseCommandHandler(
    ILoadCaseRepository repository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : PutCommandHandlerBase<
        LoadCaseId,
        Domain.PhysicalModel.LoadCases.LoadCase,
        PutLoadCaseCommand,
        LoadCase
    >(repository, unitOfWork)
{
    protected override Domain.PhysicalModel.LoadCases.LoadCase ToDomainObject(
        PutLoadCaseCommand putCommand
    ) => putCommand.ToDomainObject();

    protected override LoadCase ToResponse(Domain.PhysicalModel.LoadCases.LoadCase entity) =>
        entity.ToResponse();
}

public readonly struct PutLoadCaseCommand : IModelResourceWithIntIdRequest<LoadCaseData>
{
    public Guid ModelId { get; init; }
    public LoadCaseData Body { get; init; }
    public string Name => this.Body.Name;
    public int Id { get; init; }

    public PutLoadCaseCommand() { }

    public PutLoadCaseCommand(ModelId modelId, LoadCase putElement1DRequest)
    {
        this.Id = putElement1DRequest.Id;
        this.ModelId = modelId;
        this.Body = putElement1DRequest;
    }
}
