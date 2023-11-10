using BeamOS.PhysicalModel.Api.Mappers;
using BeamOS.PhysicalModel.Application.Models.Commands;
using BeamOS.PhysicalModel.Application.Nodes.Commands;
using BeamOS.PhysicalModel.Contracts.Node;
using FastEndpoints;

namespace BeamOS.PhysicalModel.Api.Nodes.Endpoints;

public class CreateNodeEndpoint(CreateNodeCommandHandler createNodeCommandHandler) : Endpoint<CreateNodeRequest, NodeResponse>
{
    public override void Configure()
    {
        this.Post("node");
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
        var command = req.ToCommand();

        var node = await createNodeCommandHandler.ExecuteAsync(command, ct);

        var response = node.ToResponse();
        await this.SendAsync(response, cancellation: ct);
    }
}
