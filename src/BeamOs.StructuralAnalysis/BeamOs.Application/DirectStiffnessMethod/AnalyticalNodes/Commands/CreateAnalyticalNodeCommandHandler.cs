using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.DirectStiffnessMethod.AnalyticalNodes.Commands;
using BeamOs.Domain.DirectStiffnessMethod.DsmNodeAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOS.DirectStiffnessMethod.Application.AnalyticalNodes.Commands;

public class CreateAnalyticalNodeCommandHandler
    : ICommandHandler<CreateAnalyticalNodeCommand, DsmNode>
{
    public Task<DsmNode> ExecuteAsync(CreateAnalyticalNodeCommand command, CancellationToken ct)
    {
        DsmNode node = command.ToDomainObject();

        //await nodeRepository.Add(node);

        return Task.FromResult(node);
    }
}

[Mapper]
public static partial class CreateAnalyticalNodeCommandMapper
{
    public static partial DsmNode ToDomainObject(this CreateAnalyticalNodeCommand command);
}
