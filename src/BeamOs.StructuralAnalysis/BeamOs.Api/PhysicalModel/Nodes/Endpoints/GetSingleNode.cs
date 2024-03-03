using BeamOs.Api.PhysicalModel.Nodes.Mappers;
using BeamOs.Contracts.PhysicalModel.Common;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOs.Infrastructure;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Api.PhysicalModel.Nodes.Endpoints;

public class GetSingleNode(BeamOsStructuralDbContext dbContext, NodeResponseMapper responseMapper)
    : Endpoint<IdRequest, NodeResponse?>
{
    public override void Configure()
    {
        this.Get("nodes/{id}");
        this.AllowAnonymous();
    }

    public override async Task<NodeResponse?> ExecuteAsync(IdRequest req, CancellationToken ct)
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
