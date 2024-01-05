using BeamOS.Common.Api;
using BeamOS.Common.Api.Interfaces;
using BeamOS.PhysicalModel.Api.MomentLoads.Mappers;
using BeamOS.PhysicalModel.Application.MomentLoads;
using BeamOS.PhysicalModel.Contracts.MomentLoad;
using BeamOS.PhysicalModel.Domain.MomentLoadAggregate;

namespace BeamOS.PhysicalModel.Api.MomentLoads.Endpoints;

public class CreateMomentLoad(
    CreateMomentLoadRequestMapper requestMapper,
    CreateMomentLoadCommandHandler createMomentLoadCommandHandler,
    MomentLoadResponseMapper responseMapper
) : BaseEndpoint, IPostEndpoint<CreateMomentLoadRequest, MomentLoadResponse>
{
    public override string Route => "moment-loads";

    public async Task<MomentLoadResponse> PostAsync(
        CreateMomentLoadRequest request,
        CancellationToken ct
    )
    {
        CreateMomentLoadCommand command = requestMapper.Map(request);

        MomentLoad node = await createMomentLoadCommandHandler.ExecuteAsync(command, ct);

        MomentLoadResponse response = responseMapper.Map(node);
        return response;
    }
}
