using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Api.Endpoints.OpenSees;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinations;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.LoadCombinations;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "load-combinations/{id}")]
[BeamOsEndpointType(Http.Delete)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class DeleteLoadCombination(
    DeleteLoadCombinationCommandHandler deleteLoadCombinationCommandHandler
) : BeamOsModelResourceQueryBaseEndpoint<ModelEntityResponse>
{
    public override async Task<Result<ModelEntityResponse>> ExecuteRequestAsync(
        ModelEntityRequest req,
        CancellationToken ct = default
    ) => await deleteLoadCombinationCommandHandler.ExecuteAsync(req, ct);
}

public sealed class DeleteLoadCombinationCommandHandler(
    ILoadCombinationRepository entityRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : DeleteModelEntityCommandHandler<
        LoadCombinationId,
        Domain.PhysicalModel.LoadCombinations.LoadCombination
    >(entityRepository, unitOfWork) { }
