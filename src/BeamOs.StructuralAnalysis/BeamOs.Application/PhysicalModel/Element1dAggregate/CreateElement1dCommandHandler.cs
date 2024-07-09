using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.Common.Interfaces.Repositories;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Application.PhysicalModel.Element1dAggregate;

public class CreateElement1dCommandHandler(
    IRepository<Element1DId, Element1D> element1DRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateElement1dCommand, Element1D>
{
    public async Task<Element1D> ExecuteAsync(CreateElement1dCommand command, CancellationToken ct)
    {
        var element = command.ToDomainObject();

        element1DRepository.Add(element);

        await unitOfWork.SaveChangesAsync(ct);

        return element;
    }
}

[Mapper]
public static partial class CreateElement1DCommandMapper
{
    public static partial Element1D ToDomainObject(this CreateElement1dCommand command);
}
