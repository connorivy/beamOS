using BeamOs.Application.Common.Interfaces;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Application.PhysicalModel.MomentLoads;

public class CreateMomentLoadCommandHandler(
    IRepository<MomentLoadId, MomentLoad> momentLoadRepository
) : ICommandHandler<CreateMomentLoadCommand, MomentLoad>
{
    public async Task<MomentLoad> ExecuteAsync(
        CreateMomentLoadCommand command,
        CancellationToken ct = default
    )
    {
        var load = command.ToDomainObject();

        await momentLoadRepository.Add(load);

        return load;
    }
}

[Mapper]
public static partial class CreateMomentLoadCommandMapper
{
    public static partial MomentLoad ToDomainObject(this CreateMomentLoadCommand command);
}
