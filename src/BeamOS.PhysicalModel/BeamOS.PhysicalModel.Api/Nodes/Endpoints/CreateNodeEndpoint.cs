using BeamOS.PhysicalModel.Api.Mappers;
using BeamOS.PhysicalModel.Application.Models.Commands;
using BeamOS.PhysicalModel.Application.Nodes.Commands;
using BeamOS.PhysicalModel.Contracts.Node;
using BeamOS.PhysicalModel.Domain.NodeAggregate;
using FastEndpoints;

namespace BeamOS.PhysicalModel.Api.Nodes.Endpoints;

public class CreateNodeEndpoint(
    CreateNodeRequestMapper requestMapper,
    CreateNodeCommandHandler createNodeCommandHandler,
    NodeResponseMapper responseMapper) : Endpoint<CreateNodeRequest, NodeResponse>
{
    public override void Configure()
    {
        this.Post("nodes");
        this.AllowAnonymous();
        this.Summary(s => s.ExampleRequest = new CreateNodeRequest(
            "00000000-0000-0000-0000-000000000000",
            0.0,
            0.0,
            10.0,
            "Foot",
            new RestraintsRequest(
                false,
                false,
                false,
                false,
                false,
                false)
            )
        );
    }

    public override async Task HandleAsync(CreateNodeRequest req, CancellationToken ct)
    {
        CreateNodeCommand command = requestMapper.Map(req);

        Node node = await createNodeCommandHandler.ExecuteAsync(command, ct);

        NodeResponse response = responseMapper.Map(node);
        await this.SendAsync(response, cancellation: ct);
    }
}
