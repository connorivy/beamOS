using BeamOS.Common.Api.Interfaces;
using BeamOS.PhysicalModel.Api.Mappers;
using BeamOS.PhysicalModel.Application.Models.Commands;
using BeamOS.PhysicalModel.Contracts.Model;
using BeamOS.PhysicalModel.Domain.ModelAggregate;
using FastEndpoints;

namespace BeamOS.PhysicalModel.Api.Models.Endpoints;

public class CreateModelEndpoint(
    CreateModelRequestMapper commandMapper,
    CreateModelCommandHandler createModelCommandHandler,
    ModelResponseMapper modelResponseMapper) : Endpoint<CreateModelRequest, ModelResponse>
{
    public override void Configure()
    {
        this.Post("models");
        this.AllowAnonymous();
        this.Summary(s => s.ExampleRequest = new CreateModelRequest(
            "Big Ol' Building",
            "Description",
            new ModelSettingsRequest(
                new UnitSettingsRequest(
                    "Inch",
                    "SquareInch",
                    "CubicInch",
                    "KilopoundForce",
                    "KilopoundForcePerInch",
                    "KilopoundForceInch")
                )
            )
        );
    }

    public override async Task HandleAsync(CreateModelRequest req, CancellationToken ct)
    {
        CreateModelCommand command = commandMapper.Map(req);

        Model model = await createModelCommandHandler.ExecuteAsync(command, ct);

        ModelResponse response = modelResponseMapper.Map(model);

        await this.SendAsync(response, cancellation: ct);
    }
}
