using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Api.Endpoints.OpenSees;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.LoadCases;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "load-cases/{id}")]
[BeamOsEndpointType(Http.Delete)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class DeleteLoadCase(DeleteLoadCaseCommandHandler deleteLoadCaseCommandHandler)
    : BeamOsModelResourceQueryBaseEndpoint<ModelEntityResponse>
{
    public override async Task<Result<ModelEntityResponse>> ExecuteRequestAsync(
        ModelEntityRequest req,
        CancellationToken ct = default
    ) => await deleteLoadCaseCommandHandler.ExecuteAsync(req, ct);
}

public sealed class DeleteLoadCaseCommandHandler(
    ILoadCaseRepository entityRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : DeleteModelEntityCommandHandler<LoadCaseId, Domain.PhysicalModel.LoadCases.LoadCase>(
        entityRepository,
        unitOfWork
    ) { }
