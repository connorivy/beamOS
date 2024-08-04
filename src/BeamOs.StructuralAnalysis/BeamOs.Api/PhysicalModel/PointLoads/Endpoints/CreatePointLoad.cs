using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Api.PhysicalModel.PointLoads.Mappers;
using BeamOs.Application.PhysicalModel.PointLoads.Commands;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.PointLoad;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate;
using FastEndpoints;

namespace BeamOs.Api.PhysicalModel.PointLoads.Endpoints;

public class CreatePointLoad(
    BeamOsFastEndpointOptions options,
    CreatePointLoadCommandHandler createPointLoadCommandHandler,
    PointLoadResponseMapper responseMapper
) : BeamOsFastEndpoint<CreatePointLoadRequest, PointLoadResponse>(options)
{
    public override Http EndpointType => Http.POST;

    public override string Route => "point-load";

    public override CreatePointLoadRequest? ExampleRequest =>
        new(
            "00000000-0000-0000-0000-000000000000",
            new UnitValueDto(55.0, "KilopoundForce"),
            new Vector3(10, 15, 20)
        );

    public override async Task<PointLoadResponse> ExecuteAsync(
        CreatePointLoadRequest req,
        CancellationToken ct
    )
    {
        PointLoad node = await createPointLoadCommandHandler.ExecuteAsync(req, ct);

        var response = responseMapper.Map(node);
        return response;
    }
}
