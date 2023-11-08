using BeamOS.PhysicalModel.Api.Mappers;
using BeamOS.PhysicalModel.Application.Models.Commands;
using BeamOS.PhysicalModel.Contracts;
using BeamOS.PhysicalModel.Domain.AnalyticalModelAggregate;
using FastEndpoints;

namespace BeamOS.PhysicalModel.Api.Endpoints;

public class CreateModelEndpoint(CreateModelCommandHandler createModelCommandHandler) : Endpoint<CreateModelRequest, ModelResponse>
{
    public override void Configure()
    {
        this.Post("model");
        this.AllowAnonymous();
    }

    public override async Task HandleAsync(CreateModelRequest req, CancellationToken ct)
    {
        CreateModelCommand command = req.ToCommand();

        AnalyticalModel model = await createModelCommandHandler.ExecuteAsync(command, ct);

        ModelResponse response = model.ToResponse();

        await this.SendAsync(response, cancellation: ct);
    }
}
