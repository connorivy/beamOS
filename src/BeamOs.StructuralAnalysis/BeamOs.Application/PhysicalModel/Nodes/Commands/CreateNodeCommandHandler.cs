using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.Common.Interfaces.Repositories;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Application.PhysicalModel.Nodes.Commands;

public class CreateNodeCommandHandler(
    IRepository<NodeId, Node> nodeRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateNodeCommand, Node>
{
    public async Task<Node> ExecuteAsync(CreateNodeCommand command, CancellationToken ct)
    {
        var node = command.ToDomainObject();

        nodeRepository.Add(node);

        await unitOfWork.SaveChangesAsync(ct);

        return node;
    }
}

[Mapper]
public static partial class CreateNodeCommandMapper
{
    public static partial Node ToDomainObject(this CreateNodeCommand command);
}
