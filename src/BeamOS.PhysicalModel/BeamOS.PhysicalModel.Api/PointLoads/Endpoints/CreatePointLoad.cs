using BeamOS.Common.Contracts;
using BeamOS.PhysicalModel.Api.PointLoads.Mappers;
using BeamOS.PhysicalModel.Application.PointLoads.Commands;
using BeamOS.PhysicalModel.Contracts.PointLoad;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate;
using FastEndpoints;

namespace BeamOS.PhysicalModel.Api.PointLoads.Endpoints;

public class CreatePointLoad(
    CreatePointLoadRequestMapper requestMapper,
    CreatePointLoadCommandHandler createPointLoadCommandHandler,
    PointLoadResponseMapper responseMapper
) : Endpoint<CreatePointLoadRequest, PointLoadResponse>
{
    public override void Configure()
    {
        this.Post("point-load");
        this.AllowAnonymous();
        this.Summary(
            s =>
                s.ExampleRequest = new CreatePointLoadRequest(
                    "00000000-0000-0000-0000-000000000000",
                    new UnitValueDTO(55.0, "KilopoundForce"),
                    new Vector3(10, 15, 20)
                )
        );
    }

    public override async Task HandleAsync(CreatePointLoadRequest req, CancellationToken ct)
    {
        CreatePointLoadCommand command = requestMapper.Map(req);

        PointLoad node = await createPointLoadCommandHandler.ExecuteAsync(command, ct);

        PointLoadResponse response = responseMapper.Map(node);
        await this.SendAsync(response, cancellation: ct);
    }
}
