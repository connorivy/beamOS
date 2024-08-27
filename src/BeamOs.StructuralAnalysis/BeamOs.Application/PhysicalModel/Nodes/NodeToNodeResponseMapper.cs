using BeamOs.Application.Common.Mappers;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Application.PhysicalModel.Nodes;

[Mapper]
internal partial class NodeToNodeResponseMapper : AbstractMapperProvidedUnits<Node, NodeResponse>
{
    [Obsolete("This is just here to make DI registration work. I'm too lazy to change it.", true)]
    public NodeToNodeResponseMapper()
        : base(null) { }

    private NodeToNodeResponseMapper(UnitSettings unitSettings)
        : base(unitSettings) { }

    public static NodeToNodeResponseMapper Create(UnitSettings unitSettings)
    {
        return new(unitSettings);
    }

    public override NodeResponse Map(Node source)
    {
        return this.ToResponse(source);
    }

    public partial NodeResponse ToResponse(Node source);
}
