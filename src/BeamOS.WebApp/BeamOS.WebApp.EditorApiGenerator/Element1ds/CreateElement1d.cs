using BeamOS.PhysicalModel.Contracts.Element1D;
using FastEndpoints;

namespace BeamOS.WebApp.EditorApiGenerator.Element1ds;

public class CreateElement1d : Endpoint<Element1DResponse, string>
{
    public override void Configure()
    {
        this.Post("element1ds");
    }

    public override Task HandleAsync(Element1DResponse req, CancellationToken ct) =>
        base.HandleAsync(req, ct);
}
