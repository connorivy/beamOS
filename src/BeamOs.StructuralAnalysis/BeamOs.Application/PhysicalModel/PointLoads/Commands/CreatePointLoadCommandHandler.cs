using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.Common.Interfaces.Repositories;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Application.PhysicalModel.PointLoads.Commands;

public class CreatePointLoadCommandHandler(IRepository<PointLoadId, PointLoad> pointLoadRepository)
    : ICommandHandler<CreatePointLoadCommand, PointLoad>
{
    public async Task<PointLoad> ExecuteAsync(CreatePointLoadCommand command, CancellationToken ct)
    {
        var load = command.ToDomainObject();

        pointLoadRepository.Add(load);

        return load;
    }
}

[Mapper]
public static partial class CreatePointLoadCommandMapper
{
    public static partial PointLoad ToDomainObject(this CreatePointLoadCommand command);
}
