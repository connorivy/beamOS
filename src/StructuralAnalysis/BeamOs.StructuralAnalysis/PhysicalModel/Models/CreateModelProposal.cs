using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Api.Endpoints.OpenSees;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Models;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "proposals")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Proposer)]
internal class CreateModelProposal(CreateModelProposalCommandHandler createProposalCommandHandler)
    : BeamOsModelResourceBaseEndpoint<ModelProposalData, ModelProposalResponse>
{
    public override async Task<Result<ModelProposalResponse>> ExecuteRequestAsync(
        ModelResourceRequest<ModelProposalData> req,
        CancellationToken ct = default
    ) => await createProposalCommandHandler.ExecuteAsync(req, ct);
}

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "proposals")]
[BeamOsEndpointType(Http.Get)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Reviewer)]
internal class GetModelProposals(GetModelProposalsQueryHandler getModelProposalQueryHandler)
    : BeamOsModelIdRequestBaseEndpoint<ICollection<ModelProposalInfo>>
{
    public override async Task<Result<ICollection<ModelProposalInfo>>> ExecuteRequestAsync(
        ModelResourceRequest req,
        CancellationToken ct = default
    ) => await getModelProposalQueryHandler.ExecuteAsync(req.ModelId, ct);
}

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "proposals/{id:int}")]
[BeamOsEndpointType(Http.Get)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Reviewer)]
internal class GetModelProposal(GetModelProposalQueryHandler getModelProposalQueryHandler)
    : BeamOsModelResourceQueryBaseEndpoint<ModelProposalResponse>
{
    public override async Task<Result<ModelProposalResponse>> ExecuteRequestAsync(
        ModelResourceWithIntIdRequest req,
        CancellationToken ct = default
    ) => await getModelProposalQueryHandler.ExecuteAsync(req, ct);
}

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "proposals/{id:int}/accept")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class AcceptModelProposal(
    AcceptModelProposalCommandHandler acceptModelProposalCommandHandler
) : BeamOsModelResourceWithIntIdBaseEndpoint<List<EntityProposal>?, ModelResponse>
{
    public override async Task<Result<ModelResponse>> ExecuteRequestAsync(
        ModelResourceWithIntIdRequest<List<EntityProposal>?> req,
        CancellationToken ct = default
    ) => await acceptModelProposalCommandHandler.ExecuteAsync(req, ct);
}

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "proposals/{id:int}/reject")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class RejectModelProposal(RejectModelProposalCommandHandler rejectProposalCommandHandler)
    : BeamOsModelResourceQueryBaseEndpoint<bool>
{
    public override async Task<Result<bool>> ExecuteRequestAsync(
        ModelResourceWithIntIdRequest req,
        CancellationToken ct = default
    ) => await rejectProposalCommandHandler.ExecuteAsync(req, ct);
}
