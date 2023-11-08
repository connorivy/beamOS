using BeamOS.PhysicalModel.Api.Mappers;
using BeamOS.PhysicalModel.Application.Models.Commands;
using BeamOS.PhysicalModel.Application.Nodes.Commands;
using BeamOS.PhysicalModel.Contracts.Nodes;
using BeamOS.PhysicalModel.Domain.AnalyticalNodeAggregate;
using FastEndpoints;

namespace BeamOS.PhysicalModel.Api.Endpoints;

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
                    false)));
    }

    public override async Task HandleAsync(CreateNodeRequest req, CancellationToken ct)
    {
        CreateNodeCommand command = req.ToCommand();

        AnalyticalNode node = await createNodeCommandHandler.ExecuteAsync(command, ct);

        NodeResponse response = node.ToResponse();
        await this.SendAsync(response, cancellation: ct);
    }
}
