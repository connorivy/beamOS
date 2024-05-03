using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.Common.Interfaces.Repositories;
using BeamOs.Application.PhysicalModel.Nodes.Interfaces;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;

namespace BeamOs.Application.PhysicalModel.PointLoads.Commands;

public class CreatePointLoadCommandHandler(
    INodeRepository nodeRepository,
    IRepository<PointLoadId, PointLoad> pointLoadRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreatePointLoadCommand, PointLoad>
{
    public async Task<PointLoad> ExecuteAsync(
        CreatePointLoadCommand command,
        CancellationToken ct = default
    )
    {
        NodeId nodeId = new NodeId(command.NodeId.Id);
        ModelId modelId = await nodeRepository.GetModelId(nodeId, ct).ConfigureAwait(false);

        PointLoad pointLoad = new(modelId, nodeId, command.Force, command.Direction);

        pointLoadRepository.Add(pointLoad);

        await unitOfWork.SaveChangesAsync(ct);

        return pointLoad;
    }
}

//[Mapper]
//public static partial class CreatePointLoadCommandMapper
//{
//    public static partial PointLoad ToDomainObject(this CreatePointLoadCommand command);
//}
