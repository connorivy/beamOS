using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.PhysicalModel.Nodes.Interfaces;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;

namespace BeamOs.Application.PhysicalModel.MomentLoads;

public class CreateMomentLoadCommandHandler(
    INodeRepository nodeRepository,
    IRepository<MomentLoadId, MomentLoad> momentLoadRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateMomentLoadCommand, MomentLoad>
{
    public async Task<MomentLoad> ExecuteAsync(
        CreateMomentLoadCommand command,
        CancellationToken ct = default
    )
    {
        NodeId nodeId = new NodeId(command.NodeId.Id);
        ModelId modelId = await nodeRepository.GetModelId(nodeId, ct).ConfigureAwait(false);

        MomentLoad momentLoad = new(modelId, nodeId, command.Torque, command.AxisDirection);

        momentLoadRepository.Add(momentLoad);

        await unitOfWork.SaveChangesAsync(ct);

        return momentLoad;
    }
}

//[Mapper]
//public static partial class CreateMomentLoadCommandMapper
//{
//    public static partial MomentLoad ToDomainObject(this CreateMomentLoadCommand command);
//}
