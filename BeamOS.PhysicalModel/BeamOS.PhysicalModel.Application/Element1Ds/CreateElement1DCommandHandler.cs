using BeamOS.Common.Application.Interfaces;
using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using BeamOS.PhysicalModel.Domain.Element1DAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Application.Element1Ds;

public class CreateElement1DCommandHandler(
    IRepository<Element1DId, Element1D> element1DRepository)
    : ICommandHandler<CreateElement1DCommand, Element1D>
{
    public async Task<Element1D> ExecuteAsync(CreateElement1DCommand command, CancellationToken ct)
    {
        Element1D element = command.ToDomainObject();

        await element1DRepository.Add(element);

        return element;
    }
}

[Mapper]
public static partial class CreateElement1DCommandMapper
{
    public static partial Element1D ToDomainObject(this CreateElement1DCommand command);
}
