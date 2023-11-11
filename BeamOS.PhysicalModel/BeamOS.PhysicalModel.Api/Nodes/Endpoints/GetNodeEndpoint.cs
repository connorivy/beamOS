using BeamOS.PhysicalModel.Api.Common.Endpoints;
using BeamOS.PhysicalModel.Api.Common.Interfaces;
using BeamOS.PhysicalModel.Application.Common.Interfaces;
using BeamOS.PhysicalModel.Contracts.Node;
using BeamOS.PhysicalModel.Domain.NodeAggregate;
using BeamOS.PhysicalModel.Domain.NodeAggregate.ValueObjects;

namespace BeamOS.PhysicalModel.Api.Nodes.Endpoints;
public class GetNodeEndpoint(
    IRepository<NodeId, Node> repository,
    IMapper<Node, NodeResponse> responseMapper) : GetAggregateRootByGuidBasedIdEndpoint<NodeId, Node, NodeResponse>(repository, responseMapper)
{
    public override void Configure()
    {
        this.Get("node/{id}");
        this.AllowAnonymous();
    }
}
