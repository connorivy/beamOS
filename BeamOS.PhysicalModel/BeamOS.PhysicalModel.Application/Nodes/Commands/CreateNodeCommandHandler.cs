using BeamOS.Common.Application.Interfaces;
using BeamOS.PhysicalModel.Domain.NodeAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Application.Nodes.Commands;
public class CreateNodeCommandHandler : ICommandHandler<CreateNodeCommand, Node>
{
    public async Task<Node> ExecuteAsync(CreateNodeCommand command, CancellationToken ct)
    {
        await Task.CompletedTask;

        Node node = command.ToDomainObject();

        // TODO : persist node

        return node;
    }
}

[Mapper]
public static partial class CreateNodeCommandMapper
{
    public static partial Node ToDomainObject(this CreateNodeCommand command);
}
