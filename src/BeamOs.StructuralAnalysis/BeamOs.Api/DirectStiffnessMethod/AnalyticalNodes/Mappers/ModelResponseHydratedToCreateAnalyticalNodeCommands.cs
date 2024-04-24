using BeamOs.Api.DirectStiffnessMethod.PointLoads.Mappers;
using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.DirectStiffnessMethod.AnalyticalNodes.Commands;
using BeamOs.Application.DirectStiffnessMethod.MomentLoads;
using BeamOs.Application.DirectStiffnessMethod.PointLoads;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOS.DirectStiffnessMethod.Api.AnalyticalNodes.Mappers;
using BeamOS.DirectStiffnessMethod.Api.MomentLoads.Mappers;

namespace BeamOs.Api.DirectStiffnessMethod.AnalyticalNodes.Mappers;

public class ModelResponseHydratedToCreateAnalyticalNodeCommands(
    PointResponseMapper pointResponseMapper,
    RestraintResponseMapper restraintResponseMapper,
    PointLoadResponseMapper pointLoadResponseMapper,
    MomentLoadResponseMapper momentLoadResponseMapper
) : IMapper<ModelResponseHydrated, List<CreateAnalyticalNodeCommand>>
{
    public List<CreateAnalyticalNodeCommand> Map(ModelResponseHydrated from)
    {
        List<CreateAnalyticalNodeCommand> commands = [];
        foreach (NodeResponse nodeResponse in from.Nodes)
        {
            // TODO : these could be slow
            List<CreatePointLoadCommand> pointLoadCommands = from.PointLoads
                .Where(p => p.NodeId == nodeResponse.Id)
                .Select(pointLoadResponseMapper.Map)
                .ToList();

            List<CreateMomentLoadCommand> momentLoadCommands = from.MomentLoads
                .Where(p => p.NodeId == nodeResponse.Id)
                .Select(momentLoadResponseMapper.Map)
                .ToList();

            commands.Add(
                new CreateAnalyticalNodeCommand(
                    nodeResponse.Id,
                    nodeResponse.ModelId,
                    pointResponseMapper.Map(nodeResponse.LocationPoint),
                    restraintResponseMapper.Map(nodeResponse.Restraint),
                    pointLoadCommands,
                    momentLoadCommands
                )
            );
        }
        return commands;
    }
}
