using BeamOS.Common.Application.Interfaces;
using BeamOS.PhysicalModel.Domain.MomentLoadAggregate;
using BeamOS.PhysicalModel.Domain.MomentLoadAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Application.MomentLoads;

public class CreateMomentLoadCommandHandler(
    IRepository<MomentLoadId, MomentLoad> momentLoadRepository
) : ICommandHandler<CreateMomentLoadCommand, MomentLoad>
{
    public async Task<MomentLoad> ExecuteAsync(
        CreateMomentLoadCommand command,
        CancellationToken ct = default
    )
    {
        MomentLoad load = command.ToDomainObject();

        await momentLoadRepository.Add(load);

        return load;
    }
}

[Mapper]
public static partial class CreateMomentLoadCommandMapper
{
    public static partial MomentLoad ToDomainObject(this CreateMomentLoadCommand command);
}
