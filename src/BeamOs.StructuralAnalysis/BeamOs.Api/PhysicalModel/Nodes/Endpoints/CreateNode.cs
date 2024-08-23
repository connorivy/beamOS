using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Api.PhysicalModel.Nodes.Mappers;
using BeamOs.Application.PhysicalModel.Nodes.Commands;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using FastEndpoints;

namespace BeamOs.Api.PhysicalModel.Nodes.Endpoints;

public class CreateNode(
    BeamOsFastEndpointOptions options,
    CreateNodeCommandHandler createNodeCommandHandler,
    NodeResponseMapper responseMapper
) : BeamOsFastEndpoint<CreateNodeRequest, NodeResponse>(options)
{
    public override Http EndpointType => Http.POST;
    public override string Route => "nodes";

    public override CreateNodeRequest? ExampleRequest { get; } =
        new CreateNodeRequest(
            "00000000-0000-0000-0000-000000000000",
            0.0,
            0.0,
            10.0,
            "Foot",
            new RestraintRequest(false, false, false, false, false, false)
        );

    public override async Task<NodeResponse> ExecuteRequestAsync(
        CreateNodeRequest req,
        CancellationToken ct
    )
    {
        Node node = await createNodeCommandHandler.ExecuteAsync(req, ct);

        return responseMapper.Map(node);
    }
}
