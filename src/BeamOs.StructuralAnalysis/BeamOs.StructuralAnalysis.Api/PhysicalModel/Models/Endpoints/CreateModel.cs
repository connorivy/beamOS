using BeamOs.Api.PhysicalModel.Models.Mappers;
using BeamOS.Common.Api.Interfaces;
using BeamOS.PhysicalModel.Contracts.Model;
using BeamOS.PhysicalModel.Domain.ModelAggregate;
using FastEndpoints;

namespace BeamOs.Api.PhysicalModel.Models.Endpoints;

public class CreateModel(
    CreateModelRequestMapper commandMapper,
    CreateModelCommandHandler createModelCommandHandler,
    ModelResponseMapper modelResponseMapper
) : Endpoint<CreateModelRequest, ModelResponse>
{
    public override void Configure()
    {
        this.Post("models");
        this.AllowAnonymous();
        this.Summary(
            s =>
                s.ExampleRequest = new CreateModelRequest(
                    "Big Ol' Building",
                    "Description",
                    new ModelSettingsRequest(
                        new UnitSettingsRequest(
                            "Inch",
                            "SquareInch",
                            "CubicInch",
                            "KilopoundForce",
                            "KilopoundForcePerInch",
                            "KilopoundForceInch"
                        )
                    )
                )
        );
    }

    public override async Task HandleAsync(CreateModelRequest req, CancellationToken ct)
    {
        var command = commandMapper.Map(req);

        Model model = await createModelCommandHandler.ExecuteAsync(command, ct);

        var response = modelResponseMapper.Map(model);

        await this.SendAsync(response, cancellation: ct);
    }
}
