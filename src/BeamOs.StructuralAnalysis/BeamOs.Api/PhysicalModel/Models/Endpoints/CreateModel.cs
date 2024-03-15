using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Api.PhysicalModel.Models.Mappers;
using BeamOs.Application.PhysicalModel.Models.Commands;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using FastEndpoints;

namespace BeamOs.Api.PhysicalModel.Models.Endpoints;

public class CreateModel(
    BeamOsFastEndpointOptions options,
    CreateModelRequestMapper commandMapper,
    CreateModelCommandHandler createModelCommandHandler,
    ModelResponseMapper modelResponseMapper
) : BeamOsFastEndpoint<CreateModelRequest, ModelResponse>(options)
{
    public override Http EndpointType => Http.POST;
    public override string Route => "/models";
    public override CreateModelRequest? ExampleRequest =>
        new(
            "Big Ol' Building",
            "Description",
            new ModelSettingsRequest(
                new UnitSettingsRequest(
                    "Inch",
                    "SquareInch",
                    "CubicInch",
                    "KilopoundForce",
                    "KilopoundForcePerInch",
                    "KilopoundForceInch",
                    "KilopoundForcePerSquareInch",
                    "InchToTheForth"
                )
            )
        );

    public override async Task<ModelResponse> ExecuteAsync(
        CreateModelRequest req,
        CancellationToken ct
    )
    {
        var command = commandMapper.Map(req);

        Model model = await createModelCommandHandler.ExecuteAsync(command, ct);

        var response = modelResponseMapper.Map(model);

        return response;
    }
}
