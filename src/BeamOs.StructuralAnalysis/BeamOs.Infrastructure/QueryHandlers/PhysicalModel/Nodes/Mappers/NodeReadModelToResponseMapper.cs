using BeamOs.Application.Common.Models;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Infrastructure.Data.Models;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Element1d.Mappers;

[Mapper]
internal partial class NodeReadModelToResponseMapper : AbstractMapper<NodeReadModel, NodeResponse>
{
    public override NodeResponse Map(NodeReadModel source) => this.ToResponse(source);

    public partial NodeResponse ToResponse(NodeReadModel source);
}
