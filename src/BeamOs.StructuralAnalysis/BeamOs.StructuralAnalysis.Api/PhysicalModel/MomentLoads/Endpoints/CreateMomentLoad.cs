using BeamOs.Api.PhysicalModel.MomentLoads.Mappers;
using BeamOS.Common.Api;
using BeamOS.PhysicalModel.Application.MomentLoads;
using BeamOS.PhysicalModel.Contracts.MomentLoad;
using BeamOS.PhysicalModel.Domain.MomentLoadAggregate;

namespace BeamOs.Api.PhysicalModel.MomentLoads.Endpoints;

public class CreateMomentLoad(
    CreateMomentLoadRequestMapper requestMapper,
    CreateMomentLoadCommandHandler createMomentLoadCommandHandler,
    MomentLoadResponseMapper responseMapper
) : BeamOsFastEndpoint<CreateMomentLoadRequest, MomentLoadResponse>
{
    public override void Configure()
    {
        this.Post("moment-loads");
        this.AllowAnonymous();
    }

    //public override string Route => "moment-loads";

    //public override EndpointType EndpointType => EndpointType.Post;

    public override async Task<MomentLoadResponse> ExecuteAsync(
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
