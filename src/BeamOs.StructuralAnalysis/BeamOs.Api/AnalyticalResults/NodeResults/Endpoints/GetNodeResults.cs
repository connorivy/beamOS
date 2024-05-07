using BeamOs.Api.PhysicalModel.Nodes.Mappers;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOs.Infrastructure;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Api.PhysicalModel.Nodes.Endpoints;

public class GetNodeResults(BeamOsStructuralDbContext dbContext, NodeResponseMapper responseMapper)
    : Endpoint<GetNodeResultsRequest, NodeResponse?[]>
{
    public override void Configure()
    {
        this.Get("node-results/{modelId}");
        this.AllowAnonymous();
    }

    public override async Task<NodeResponse?[]> ExecuteAsync(
        GetNodeResultsRequest req,
        CancellationToken ct
    )
    {
        NodeId expectedId = new(Guid.Parse(req.Id));
        Node? element = await dbContext
            .Nodes
            .FirstAsync(n => n.Id == expectedId, cancellationToken: ct);

        if (element is null)
        {
            return null;
        }

        NodeResponse? response = responseMapper.Map(element);

        return response;
    }
}
