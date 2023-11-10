using BeamOS.PhysicalModel.Api.PointLoads.Mappers;
using BeamOS.PhysicalModel.Application.PointLoads.Commands;
using BeamOS.PhysicalModel.Contracts.Common;
using BeamOS.PhysicalModel.Contracts.PointLoad;
using FastEndpoints;

namespace BeamOS.PhysicalModel.Api.PointLoads.Endpoints;
public class CreatePointLoadEndpoint(CreatePointLoadCommandHandler createPointLoadCommandHandler)
    : Endpoint<CreatePointLoadRequest, PointLoadResponse>
{
    public override void Configure()
    {
        this.Post("point-load");
        this.AllowAnonymous();
        this.Summary(s => s.ExampleRequest = new CreatePointLoadRequest(
            "00000000-0000-0000-0000-000000000000",
            new UnitValueDTO(55.0, "KilopoundForce"),
            new Vector3(10, 15, 20)
            )
        );
    }

    public override async Task HandleAsync(CreatePointLoadRequest req, CancellationToken ct)
    {
        var command = req.ToCommand();

        var node = await createPointLoadCommandHandler.ExecuteAsync(command, ct);

        var response = node.ToResponse();
        await this.SendAsync(response, cancellation: ct);
    }
}
