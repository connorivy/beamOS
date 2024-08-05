using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Api.PhysicalModel.MomentLoads.Mappers;
using BeamOs.Application.PhysicalModel.MomentLoads;
using BeamOs.Contracts.PhysicalModel.MomentLoad;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate;
using FastEndpoints;

namespace BeamOs.Api.PhysicalModel.MomentLoads.Endpoints;

public class CreateMomentLoad(
    BeamOsFastEndpointOptions options,
    CreateMomentLoadRequestMapper requestMapper,
    CreateMomentLoadCommandHandler createMomentLoadCommandHandler,
    MomentLoadResponseMapper responseMapper
) : BeamOsFastEndpoint<CreateMomentLoadRequest, MomentLoadResponse>(options)
{
    public override string Route => "moment-loads";

    public override Http EndpointType => Http.POST;

    public override async Task<MomentLoadResponse> ExecuteRequestAsync(
        CreateMomentLoadRequest request,
        CancellationToken ct
    )
    {
        var command = requestMapper.Map(request);

        MomentLoad node = await createMomentLoadCommandHandler.ExecuteAsync(command, ct);

        var response = responseMapper.Map(node);
        return response;
    }
}
