using BeamOS.Common.Application.Interfaces;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOS.DirectStiffnessMethod.Application.AnalyticalNodes.Commands;

public class CreateAnalyticalNodeCommandHandler(IRepository<AnalyticalNodeId, AnalyticalNode> nodeRepository)
    : ICommandHandler<CreateAnalyticalNodeCommand, AnalyticalNode>
{
    public async Task<AnalyticalNode> ExecuteAsync(CreateAnalyticalNodeCommand command, CancellationToken ct)
    {
        AnalyticalNode node = command.ToDomainObject();

        await nodeRepository.Add(node);

        return node;
    }
}

[Mapper]
public static partial class CreateAnalyticalNodeCommandMapper
{
    public static partial AnalyticalNode ToDomainObject(this CreateAnalyticalNodeCommand command);
}
