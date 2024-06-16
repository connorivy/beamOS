using FastEndpoints;

namespace BeamOS.WebApp.EditorApiGenerator.Utils;

public class Clear : Endpoint<EmptyRequest, string>
{
    public override void Configure()
    {
        this.Post("clear");
    }

    public override Task HandleAsync(EmptyRequest req, CancellationToken ct) =>
        base.HandleAsync(req, ct);
}
