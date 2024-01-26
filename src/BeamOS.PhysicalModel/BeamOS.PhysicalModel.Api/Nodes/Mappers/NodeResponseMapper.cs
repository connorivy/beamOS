using BeamOS.Common.Api.Interfaces;
using BeamOS.PhysicalModel.Contracts.Node;
using BeamOS.PhysicalModel.Domain.NodeAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Api.Mappers;

[Mapper]
public partial class NodeResponseMapper : IMapper<Node, NodeResponse>
{
    public NodeResponse Map(Node from) => this.ToResponse(from);

    public partial NodeResponse ToResponse(Node model);
}
