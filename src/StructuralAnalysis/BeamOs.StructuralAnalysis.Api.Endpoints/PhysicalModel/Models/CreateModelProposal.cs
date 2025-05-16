using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Api.Endpoints.OpenSees;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Models;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "proposals")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Proposer)]
public class CreateModelProposal(CreateProposalCommandHandler putModelCommandHandler)
    : BeamOsModelResourceBaseEndpoint<
        ModelResourceRequest<ModelProposalData>,
        ModelProposalData,
        ModelProposal
    >
{
    public override async Task<Result<ModelProposal>> ExecuteRequestAsync(
        ModelResourceRequest<ModelProposalData> req,
        CancellationToken ct = default
    ) => await putModelCommandHandler.ExecuteAsync(req, ct);
}

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "proposals/{id:int}")]
[BeamOsEndpointType(Http.Get)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Reviewer)]
public class GetModelProposal(GetModelProposalQueryHandler getModelProposalQueryHandler)
    : BeamOsModelResourceQueryBaseEndpoint<ModelProposal>
{
    public override async Task<Result<ModelProposal>> ExecuteRequestAsync(
        ModelEntityRequest req,
        CancellationToken ct = default
    ) => await getModelProposalQueryHandler.ExecuteAsync(req, ct);
}
