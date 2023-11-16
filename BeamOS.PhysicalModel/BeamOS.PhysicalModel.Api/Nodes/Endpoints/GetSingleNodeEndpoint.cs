using BeamOS.PhysicalModel.Api.Common.Interfaces;
using BeamOS.PhysicalModel.Api.Data;
using BeamOS.PhysicalModel.Contracts.Common;
using BeamOS.PhysicalModel.Contracts.Node;
using BeamOS.PhysicalModel.Domain.NodeAggregate;
using FastEndpoints;

namespace BeamOS.PhysicalModel.Api.Nodes.Endpoints;
public class GetSingleNodeEndpoint(
    PhysicalModelDbContext dbContext,
    IMapper<Node, NodeResponse> responseMapper) : Endpoint<IdRequest, NodeResponse?>
{
    public override void Configure()
    {
        this.Get("nodes/{id}");
        this.AllowAnonymous();
    }

    public override Task<NodeResponse?> ExecuteAsync(IdRequest req, CancellationToken ct)
    {

        Node? element = dbContext.Nodes.Where(e => e.Id.Value == Guid.Parse(req.Id))
            .FirstOrDefault();

        if (element is null)
        {
            return Task.FromResult<NodeResponse?>(null);
        }

        NodeResponse? response = responseMapper.Map(element);

        return Task.FromResult<NodeResponse?>(response);
    }
}
