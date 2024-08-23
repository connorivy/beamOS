using BeamOs.Application.Common.Mappers;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.Nodes.Mappers;

[Mapper]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
public partial class NodeResponseMapper : IMapper<Node, NodeResponse>
{
    public NodeResponse Map(Node from) => this.ToResponse(from);

    public partial NodeResponse ToResponse(Node model);
}
