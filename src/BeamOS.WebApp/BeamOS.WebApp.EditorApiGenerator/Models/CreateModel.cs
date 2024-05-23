using BeamOs.Contracts.PhysicalModel.Model;
using FastEndpoints;

namespace BeamOS.WebApp.EditorApiGenerator.Models;

public class CreateModel : Endpoint<ModelResponse, string>
{
    public override void Configure()
    {
        this.Post("models");
    }

    public override Task HandleAsync(ModelResponse req, CancellationToken ct) =>
        base.HandleAsync(req, ct);
}
