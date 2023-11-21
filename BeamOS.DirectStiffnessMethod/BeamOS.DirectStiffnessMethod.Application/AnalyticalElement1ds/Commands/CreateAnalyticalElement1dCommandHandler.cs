using BeamOS.Common.Application.Interfaces;
using BeamOS.DirectStiffnessMethod.Application.AnalyticalElement1ds.Commands;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalElement1DAggregate;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalElement1DAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOS.DirectStiffnessMethod.Application.AnalyticalNodes.Commands;

public class CreateAnalyticalElement1dCommandHandler(IRepository<AnalyticalElement1DId, AnalyticalElement1D> Element1DRepository)
    : ICommandHandler<CreateAnalyticalElement1dCommand, AnalyticalElement1D>
{
    public async Task<AnalyticalElement1D> ExecuteAsync(CreateAnalyticalElement1dCommand command, CancellationToken ct)
    {
        AnalyticalElement1D element1D = command.ToDomainObject();

        await Element1DRepository.Add(element1D);

        return element1D;
    }
}

[Mapper]
public static partial class CreateAnalyticalElement1DCommandMapper
{
    public static partial AnalyticalElement1D ToDomainObject(this CreateAnalyticalElement1DCommand command);
}
