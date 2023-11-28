using BeamOS.Common.Application.Interfaces;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalElement1DAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOS.DirectStiffnessMethod.Application.AnalyticalElement1ds.Commands;
public class CreateAnalyticalElement1dGivenEntitiesCommandHandler
    : ICommandHandler<CreateAnalyticalElement1dGivenEntitiesCommand, AnalyticalElement1D>
{
    public Task<AnalyticalElement1D> ExecuteAsync(CreateAnalyticalElement1dGivenEntitiesCommand command, CancellationToken ct)
    {
        AnalyticalElement1D element1D = command.ToDomainObject();

        //await element1DRepository.Add(element1D);

        return Task.FromResult(element1D);
    }
}

[Mapper]
public static partial class CreateAnalyticalElement1dGivenEntitiesCommandMapper
{
    public static partial AnalyticalElement1D ToDomainObject(this CreateAnalyticalElement1dGivenEntitiesCommand command);
}
