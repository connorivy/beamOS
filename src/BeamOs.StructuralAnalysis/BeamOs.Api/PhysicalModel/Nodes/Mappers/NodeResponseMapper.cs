using BeamOs.Api.Common.Interfaces;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.Nodes.Mappers;

[Mapper]
public partial class NodeResponseMapper : IMapper<Node, NodeResponse>
{
    public NodeResponse Map(Node from) => this.ToResponse(from);

    public partial NodeResponse ToResponse(Node model);
}
