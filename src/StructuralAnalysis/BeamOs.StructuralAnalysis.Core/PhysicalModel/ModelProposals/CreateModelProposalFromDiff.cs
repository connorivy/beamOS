using BeamOs.Common.Api;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Models;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "proposals/from-diff")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Reviewer)]
[BeamOsTag(BeamOsTags.AI)]
internal class CreateModelProposalFromDiff(
    CreateModelProposalFromDiffCommandHandler createModelProposalFromDiffCommandHandler
) : BeamOsModelResourceBaseEndpoint<ModelDiffData, ModelProposalResponse>
{
    public override async Task<Result<ModelProposalResponse>> ExecuteRequestAsync(
        ModelResourceRequest<ModelDiffData> req,
        CancellationToken ct = default
    ) => await createModelProposalFromDiffCommandHandler.ExecuteAsync(req, ct);
}

internal class CreateModelProposalFromDiffCommandHandler(
    CreateModelProposalCommandHandler createModelProposalCommandHandler
) : ICommandHandler<ModelResourceRequest<ModelDiffData>, ModelProposalResponse>
{
    public async Task<Result<ModelProposalResponse>> ExecuteAsync(
        ModelResourceRequest<ModelDiffData> command,
        CancellationToken ct = default
    )
    {
        throw new NotImplementedException(
            "need to implement CreateModelProposalFromDiffCommandHandler"
        );
        // build up a ModelProposalData from the diff
        // call await createModelProposalCommandHandler.ExecuteAsync with that data
    }
}
