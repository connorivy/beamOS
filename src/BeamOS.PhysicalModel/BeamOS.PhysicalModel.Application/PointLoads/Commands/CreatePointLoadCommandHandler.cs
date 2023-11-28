using BeamOS.Common.Application.Interfaces;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Application.PointLoads.Commands;

public class CreatePointLoadCommandHandler(IRepository<PointLoadId, PointLoad> pointLoadRepository)
    : ICommandHandler<CreatePointLoadCommand, PointLoad>
{
    public async Task<PointLoad> ExecuteAsync(CreatePointLoadCommand command, CancellationToken ct)
    {
        PointLoad load = command.ToDomainObject();

        await pointLoadRepository.Add(load);

        return load;
    }
}

[Mapper]
public static partial class CreatePointLoadCommandMapper
{
    public static partial PointLoad ToDomainObject(this CreatePointLoadCommand command);
}
