using BeamOS.Common.Application.Interfaces;
using BeamOS.PhysicalModel.Domain.AnalyticalNodeAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Application.Nodes.Commands;
public class CreateNodeCommandHandler : ICommandHandler<CreateNodeCommand, AnalyticalNode>
{
    public async Task<AnalyticalNode> ExecuteAsync(CreateNodeCommand command, CancellationToken ct)
    {
        await Task.CompletedTask;

        AnalyticalNode node = command.ToDomainObject();

        // TODO : persist node

        return node;
    }
}

[Mapper]
public static partial class CreateNodeCommandMapper
{
    public static partial AnalyticalNode ToDomainObject(this CreateNodeCommand command);
}
