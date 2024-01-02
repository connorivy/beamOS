using BeamOS.PhysicalModel.Contracts.Node;
using FastEndpoints;

namespace BeamOS.WebApp.EditorApi.Nodes;

public class CreateNode : Endpoint<NodeResponse, string>
{
    public override void Configure()
    {
        this.Post("nodes");
    }

    public override Task HandleAsync(NodeResponse req, CancellationToken ct) =>
        base.HandleAsync(req, ct);
}
