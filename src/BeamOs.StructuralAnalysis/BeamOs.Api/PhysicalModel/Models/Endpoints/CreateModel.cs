using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Api.PhysicalModel.Models.Mappers;
using BeamOs.Application.PhysicalModel.Models.Commands;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using FastEndpoints;

namespace BeamOs.Api.PhysicalModel.Models.Endpoints;

public class CreateModel(
    BeamOsFastEndpointOptions options,
    CreateModelRequestMapper commandMapper,
    CreateModelCommandHandler createModelCommandHandler,
    ModelResponseMapper modelResponseMapper
) : BeamOsFastEndpoint<CreateModelRequest, ModelResponse?>(options)
{
    public override Http EndpointType => Http.POST;
    public override string Route => "/models";
    public override CreateModelRequest? ExampleRequest =>
        new(
            "Big Ol' Building",
            "Description",
            new PhysicalModelSettingsDto(
                new UnitSettingsDtoVerbose(
                    "Inch",
                    "SquareInch",
                    "CubicInch",
                    "InchToTheFourth",
                    "KilopoundForce",
                    "KilopoundForceInch",
                    "KilopoundForcePerInch",
                    "KilopoundForcePerSquareInch"
                )
            )
        );

    public override async Task<ModelResponse?> ExecuteAsync(
        CreateModelRequest req,
        CancellationToken ct
    )
    {
        var command = commandMapper.Map(req);

        Model? model = await createModelCommandHandler.ExecuteAsync(command, ct);

        if (model is null)
        {
            return null;
        }

        var response = modelResponseMapper.Map(model);

        return response;
    }
}
