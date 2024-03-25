using BeamOs.Contracts.PhysicalModel.Model;
using FastEndpoints;

namespace BeamOS.WebApp.EditorApiGenerator.Models;

public class CreateModelHydrated : Endpoint<ModelResponseHydrated, string>
{
    public override void Configure()
    {
        this.Post("models");
    }

    public override Task HandleAsync(ModelResponseHydrated req, CancellationToken ct) =>
        base.HandleAsync(req, ct);
}
