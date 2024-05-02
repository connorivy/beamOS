using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.Common.Interfaces.Repositories;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Application.PhysicalModel.PointLoads.Commands;

public class CreatePointLoadCommandHandler(
    IRepository<PointLoadId, PointLoad> pointLoadRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreatePointLoadCommand, PointLoad>
{
    public async Task<PointLoad> ExecuteAsync(
        CreatePointLoadCommand command,
        CancellationToken ct = default
    )
    {
        var load = command.ToDomainObject();

        pointLoadRepository.Add(load);

        await unitOfWork.SaveChangesAsync(ct);

        return load;
    }
}

[Mapper]
public static partial class CreatePointLoadCommandMapper
{
    public static partial PointLoad ToDomainObject(this CreatePointLoadCommand command);
}
