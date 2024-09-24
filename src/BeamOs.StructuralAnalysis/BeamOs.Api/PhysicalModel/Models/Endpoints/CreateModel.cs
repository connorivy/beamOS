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
    CreateModelCommandHandler createModelCommandHandler,
    ModelResponseMapper modelResponseMapper
) : BeamOsFastEndpoint<CreateModelRequest, ModelResponse?>(options)
{
    public override Http EndpointType => Http.POST;
    public override string Route => "/models";
    public override CreateModelRequest? ExampleRequest =>
        new()
        {
            Name = "Big Ol' Building",
            Description = "Description",
            Settings = new PhysicalModelSettings(UnitSettingsContract.K_IN)
        };

    public override void ConfigureAuthentication() { }

    public override async Task<ModelResponse?> ExecuteRequestAsync(
        CreateModelRequest req,
        CancellationToken ct
    )
    {
        Model? model = await createModelCommandHandler.ExecuteAsync(req, ct);

        if (model is null)
        {
            return null;
        }

        var response = modelResponseMapper.Map(model);

        return response;
    }
}
