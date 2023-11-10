using BeamOS.Common.Application.Interfaces;
using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Application.Element1Ds;
public class CreateElement1DCommandHandler : ICommandHandler<CreateElement1DCommand, Element1D>
{
    public async Task<Element1D> ExecuteAsync(CreateElement1DCommand command, CancellationToken ct)
    {
        await Task.CompletedTask;

        Element1D element = command.ToDomainObject();

        // TODO : persist element

        return element;
    }
}

[Mapper]
public static partial class CreateElement1DCommandMapper
{
    public static partial Element1D ToDomainObject(this CreateElement1DCommand command);
}
