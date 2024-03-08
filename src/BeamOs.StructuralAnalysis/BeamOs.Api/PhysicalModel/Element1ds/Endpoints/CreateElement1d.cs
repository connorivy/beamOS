using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Api.PhysicalModel.Element1ds.Mappers;
using BeamOs.Application.PhysicalModel.Element1dAggregate;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using FastEndpoints;

namespace BeamOs.Api.PhysicalModel.Element1ds.Endpoints;

public class CreateElement1d(
    BeamOsFastEndpointOptions options,
    CreateElement1dCommandHandler createElement1dCommandHandler,
    Element1DResponseMapper responseMapper
) : BeamOsFastEndpoint<CreateElement1dRequest, Element1DResponse>(options)
{
    public override Http EndpointType => Http.POST;

    public override string Route => "element1Ds";

    public override CreateElement1dRequest? ExampleRequest =>
        new(
            "00000000-0000-0000-0000-000000000000",
            "00000000-0000-0000-0000-000000000001",
            "00000000-0000-0000-0000-000000000002",
            "00000000-0000-0000-0000-000000000003",
            "00000000-0000-0000-0000-000000000004"
        );

    public override async Task<Element1DResponse> ExecuteAsync(
        CreateElement1dRequest req,
        CancellationToken ct
    )
    {
        var command = req.ToCommand();

        Element1D element1D = await createElement1dCommandHandler.ExecuteAsync(command, ct);

        var response = responseMapper.Map(element1D);
        return response;
    }
}
