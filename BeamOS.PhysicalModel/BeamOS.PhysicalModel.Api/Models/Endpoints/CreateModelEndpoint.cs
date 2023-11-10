using BeamOS.PhysicalModel.Api.Mappers;
using BeamOS.PhysicalModel.Application.Models.Commands;
using BeamOS.PhysicalModel.Contracts.Model;
using BeamOS.PhysicalModel.Domain.AnalyticalModelAggregate;
using FastEndpoints;

namespace BeamOS.PhysicalModel.Api.Models.Endpoints;

public class CreateModelEndpoint(CreateModelCommandHandler createModelCommandHandler) : Endpoint<CreateModelRequest, ModelResponse>
{
    public override void Configure()
    {
        this.Post("model");
        this.AllowAnonymous();
    }

    public override async Task HandleAsync(CreateModelRequest req, CancellationToken ct)
    {
        var command = req.ToCommand();

        var model = await createModelCommandHandler.ExecuteAsync(command, ct);

        var response = model.ToResponse();

        await this.SendAsync(response, cancellation: ct);
    }
}
