using BeamOS.Common.Api.Interfaces;
using BeamOS.DirectStiffnessMethod.Api.PointLoads.Mappers;
using BeamOS.DirectStiffnessMethod.Application.AnalyticalNodes.Commands;
using BeamOS.DirectStiffnessMethod.Application.PointLoads;
using BeamOS.PhysicalModel.Contracts.Model;
using BeamOS.PhysicalModel.Contracts.Node;

namespace BeamOS.DirectStiffnessMethod.Api.AnalyticalNodes.Mappers;

public class ModelResponseHydratedToCreateAnalyticalNodeCommands(
    PointResponseMapper pointResponseMapper,
    RestraintResponseMapper restraintResponseMapper,
    PointLoadResponseMapper pointLoadResponseMapper
) : IMapper<ModelResponseHydrated, List<CreateAnalyticalNodeCommand>>
{
    public List<CreateAnalyticalNodeCommand> Map(ModelResponseHydrated from)
    {
        List<CreateAnalyticalNodeCommand> commands = [];
        foreach (NodeResponse nodeResponse in from.Nodes)
        {
            // TODO : this could be slow
            List<CreatePointLoadCommand> pointLoadCommands = from.PointLoads
                .Where(p => p.NodeId == nodeResponse.Id)
                .Select(pointLoadResponseMapper.Map)
                .ToList();

            commands.Add(
                new CreateAnalyticalNodeCommand(
                    nodeResponse.Id,
                    nodeResponse.ModelId,
                    pointResponseMapper.Map(nodeResponse.LocationPoint),
                    restraintResponseMapper.Map(nodeResponse.Restraint),
                    pointLoadCommands
                )
            );
        }
        return commands;
    }
}
