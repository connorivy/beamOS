using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.Common.Interfaces.Repositories;
using BeamOs.Application.Common.Mappers;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Application.PhysicalModel.Nodes.Interfaces;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.PhysicalModel.PointLoad;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;

namespace BeamOs.Application.PhysicalModel.PointLoads.Commands;

public class CreatePointLoadCommandHandler(
    INodeRepository nodeRepository,
    IRepository<PointLoadId, PointLoad> pointLoadRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreatePointLoadRequest, PointLoad>
{
    public async Task<PointLoad> ExecuteAsync(
        CreatePointLoadRequest command,
        CancellationToken ct = default
    )
    {
        NodeId nodeId = new(Guid.Parse(command.NodeId));
        ModelId modelId = await nodeRepository.GetModelId(nodeId, ct).ConfigureAwait(false);

        PointLoad pointLoad =
            new(
                modelId,
                nodeId,
                command.Force.MapToForce(),
                Vector3ToFromMathnetVector3d.MapVector3(command.Direction)
            );

        pointLoadRepository.Add(pointLoad);

        await unitOfWork.SaveChangesAsync(ct);

        return pointLoad;
    }
}
