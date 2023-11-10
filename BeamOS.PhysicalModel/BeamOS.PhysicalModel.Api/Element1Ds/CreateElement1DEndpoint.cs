using BeamOS.PhysicalModel.Api.Mappers;
using BeamOS.PhysicalModel.Application.Element1Ds;
using BeamOS.PhysicalModel.Application.Models.Commands;
using BeamOS.PhysicalModel.Contracts.Element1D;
using FastEndpoints;

namespace BeamOS.PhysicalModel.Api.Element1Ds;

public class CreateElement1DEndpoint(CreateElement1DCommandHandler createNodeCommandHandler) : Endpoint<CreateElement1DRequest, Element1DResponse>
{
    public override void Configure()
    {
        this.Post("element1D");
        this.AllowAnonymous();
        this.Summary(s => s.ExampleRequest = new CreateElement1DRequest(
            "00000000-0000-0000-0000-000000000000",
            "00000000-0000-0000-0000-000000000001",
            "00000000-0000-0000-0000-000000000002",
            "00000000-0000-0000-0000-000000000003",
            "00000000-0000-0000-0000-000000000004")
            );
    }

    public override async Task HandleAsync(CreateElement1DRequest req, CancellationToken ct)
    {
        var command = req.ToCommand();

        var node = await createNodeCommandHandler.ExecuteAsync(command, ct);

        var response = node.ToResponse();
        await this.SendAsync(response, cancellation: ct);
    }
}
