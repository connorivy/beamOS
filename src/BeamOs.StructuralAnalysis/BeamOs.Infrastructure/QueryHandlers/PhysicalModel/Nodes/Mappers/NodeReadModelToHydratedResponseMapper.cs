using BeamOs.Application.Common.Models;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Infrastructure.Data.Models;
using BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Nodes.Mappers;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Element1d.Mappers;

internal partial class NodeReadModelToHydratedResponseMapper(
    IFlattenedLocationPointToLocationPointResponseMapper pointResponseMapper,
    NodeReadModelToRestraintMapper restraintResponseMapper
) : AbstractMapper<NodeReadModel, NodeResponse>
{
    public override NodeResponse Map(NodeReadModel source)
    {
        return new(
            source.Id.ToString(),
            source.ModelId.ToString(),
            pointResponseMapper.Map(source),
            restraintResponseMapper.Map(source)
        );
    }
}
