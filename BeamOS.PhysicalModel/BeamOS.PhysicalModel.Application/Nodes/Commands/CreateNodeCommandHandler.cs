using BeamOS.Common.Application.Interfaces;
using BeamOS.PhysicalModel.Domain.NodeAggregate;
using BeamOS.PhysicalModel.Domain.NodeAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Application.Nodes.Commands;
public class CreateNodeCommandHandler(IRepository<NodeId, Node> nodeRepository)
    : ICommandHandler<CreateNodeCommand, Node>
{
    public async Task<Node> ExecuteAsync(CreateNodeCommand command, CancellationToken ct)
    {
        Node node = command.ToDomainObject();

        await nodeRepository.Add(node);

        return node;
    }
}

[Mapper]
public static partial class CreateNodeCommandMapper
{
    public static partial Node ToDomainObject(this CreateNodeCommand command);
}
