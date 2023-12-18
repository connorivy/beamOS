using BeamOS.Common.Api.Interfaces;
using BeamOS.PhysicalModel.Api.Element1Ds.Mappers;
using BeamOS.PhysicalModel.Api.Mappers;
using BeamOS.PhysicalModel.Application.Element1Ds;
using BeamOS.PhysicalModel.Contracts.Element1D;
using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using FastEndpoints;

namespace BeamOS.PhysicalModel.Api.Element1Ds.Endpoints;

public class CreateElement1d(
    CreateElement1DCommandHandler createElement1dCommandHandler,
    Element1DResponseMapper responseMapper
) : Endpoint<CreateElement1DRequest, Element1DResponse>
{
    public override void Configure()
    {
        this.Post("element1Ds");
        this.AllowAnonymous();
        this.Summary(
            s =>
                s.ExampleRequest = new CreateElement1DRequest(
                    "00000000-0000-0000-0000-000000000000",
                    "00000000-0000-0000-0000-000000000001",
                    "00000000-0000-0000-0000-000000000002",
                    "00000000-0000-0000-0000-000000000003",
                    "00000000-0000-0000-0000-000000000004"
                )
        );
    }

    public override async Task HandleAsync(CreateElement1DRequest req, CancellationToken ct)
    {
        CreateElement1DCommand command = req.ToCommand();

        Element1D element1D = await createElement1dCommandHandler.ExecuteAsync(command, ct);

        Element1DResponse response = responseMapper.Map(element1D);
        await this.SendAsync(response, cancellation: ct);
    }
}
