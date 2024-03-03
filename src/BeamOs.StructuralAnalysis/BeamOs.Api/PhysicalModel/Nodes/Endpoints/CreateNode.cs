using BeamOs.Api.PhysicalModel.Nodes.Mappers;
using BeamOs.Application.PhysicalModel.Nodes.Commands;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using FastEndpoints;

namespace BeamOs.Api.PhysicalModel.Nodes.Endpoints;

public class CreateNode(
    CreateNodeRequestMapper requestMapper,
    CreateNodeCommandHandler createNodeCommandHandler,
    NodeResponseMapper responseMapper
) : Endpoint<CreateNodeRequest, NodeResponse>
{
    public override void Configure()
    {
        this.Post("nodes");
        this.AllowAnonymous();
        this.Summary(
            s =>
                s.ExampleRequest = new CreateNodeRequest(
                    "00000000-0000-0000-0000-000000000000",
                    0.0,
                    0.0,
                    10.0,
                    "Foot",
                    new RestraintsRequest(false, false, false, false, false, false)
                )
        );
    }

    public override async Task HandleAsync(CreateNodeRequest req, CancellationToken ct)
    {
        var command = requestMapper.Map(req);

        Node node = await createNodeCommandHandler.ExecuteAsync(command, ct);

        var response = responseMapper.Map(node);
        await this.SendAsync(response, cancellation: ct);
    }
}
