using BeamOS.Common.Api;
using BeamOS.PhysicalModel.Api.MomentLoads.Mappers;
using BeamOS.PhysicalModel.Application.MomentLoads;
using BeamOS.PhysicalModel.Contracts.MomentLoad;
using BeamOS.PhysicalModel.Domain.MomentLoadAggregate;

namespace BeamOS.PhysicalModel.Api.MomentLoads.Endpoints;

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
        CreateMomentLoadCommand command = requestMapper.Map(request);

        MomentLoad node = await createMomentLoadCommandHandler.ExecuteAsync(command, ct);

        MomentLoadResponse response = responseMapper.Map(node);
        return response;
    }
}
