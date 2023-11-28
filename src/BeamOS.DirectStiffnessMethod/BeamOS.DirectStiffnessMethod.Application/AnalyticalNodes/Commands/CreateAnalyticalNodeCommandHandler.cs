using BeamOS.Common.Application.Interfaces;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOS.DirectStiffnessMethod.Application.AnalyticalNodes.Commands;

public class CreateAnalyticalNodeCommandHandler : ICommandHandler<CreateAnalyticalNodeCommand, AnalyticalNode>
{
    public Task<AnalyticalNode> ExecuteAsync(CreateAnalyticalNodeCommand command, CancellationToken ct)
    {
        AnalyticalNode node = command.ToDomainObject();

        //await nodeRepository.Add(node);

        return Task.FromResult(node);
    }
}

[Mapper]
public static partial class CreateAnalyticalNodeCommandMapper
{
    public static partial AnalyticalNode ToDomainObject(this CreateAnalyticalNodeCommand command);
}
