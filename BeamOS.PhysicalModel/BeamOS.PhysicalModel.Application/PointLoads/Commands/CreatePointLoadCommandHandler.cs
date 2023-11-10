using BeamOS.Common.Application.Interfaces;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Application.PointLoads.Commands;

public class CreatePointLoadCommandHandler : ICommandHandler<CreatePointLoadCommand, PointLoad>
{
    public async Task<PointLoad> ExecuteAsync(CreatePointLoadCommand command, CancellationToken ct)
    {
        await Task.CompletedTask;

        PointLoad load = command.ToDomainObject();

        // TODO : persist load

        return load;
    }
}

[Mapper]
public static partial class CreatePointLoadCommandMapper
{
    public static partial PointLoad ToDomainObject(this CreatePointLoadCommand command);
}
