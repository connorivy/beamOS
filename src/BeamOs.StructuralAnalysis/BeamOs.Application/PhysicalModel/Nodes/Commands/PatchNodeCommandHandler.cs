using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.PhysicalModel.Common;
using BeamOs.Application.PhysicalModel.Models;
using BeamOs.Application.PhysicalModel.Nodes.Interfaces;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;

namespace BeamOs.Application.PhysicalModel.Nodes.Commands;

public class PatchNodeCommandHandler(
    INodeRepository nodeRepository,
    IModelRepository modelRepository,
    PatchRestraintCommandHandler patchRestraintCommandHandler,
    PatchPointCommandHandler patchPointCommandHandler,
    IUnitOfWork unitOfWork
) : ICommandHandler<PatchNodeRequest, NodeResponse>
{
    public async Task<NodeResponse> ExecuteAsync(PatchNodeRequest command, CancellationToken ct)
    {
        Node node = await nodeRepository.GetById(new NodeId(Guid.Parse(command.NodeId)), ct);

        if (node is null)
        {
            throw new Exception($"Could not find node with Id {node.Id}");
        }

        if (command.Restraint is not null)
        {
            PatchRestraintCommand patchRestraintCommand =
                new(PatchRequest: command.Restraint, Restraint: node.Restraint);

            node.Restraint = patchRestraintCommandHandler.Execute(patchRestraintCommand);
        }

        if (command.LocationPoint is not null)
        {
            PatchPointCommand patchPointCommand =
                new(PatchRequest: command.LocationPoint, Point: node.LocationPoint);

            node.LocationPoint = patchPointCommandHandler.Execute(patchPointCommand);
        }

        await unitOfWork.SaveChangesAsync(ct);

        var unitSettings = await modelRepository.GetUnits(node.ModelId, CancellationToken.None);
        var mapper = NodeToNodeResponseMapper.Create(unitSettings);

        return mapper.Map(node);
    }
}
