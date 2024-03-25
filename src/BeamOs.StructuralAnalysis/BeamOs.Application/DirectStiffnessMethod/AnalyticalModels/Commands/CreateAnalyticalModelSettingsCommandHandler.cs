using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.DirectStiffnessMethod.AnalyticalModels.Commands;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalModelAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOS.DirectStiffnessMethod.Application.AnalyticalModels.Commands;

public class CreateAnalyticalModelSettingsCommandHandler
    : ICommandHandler<AnalyticalModelSettingsCommand, AnalyticalModelSettings>
{
    public Task<AnalyticalModelSettings> ExecuteAsync(
        AnalyticalModelSettingsCommand command,
        CancellationToken ct
    )
    {
        AnalyticalModelSettings settings = command.ToDomainObject();

        return Task.FromResult(settings);
    }
}

[Mapper]
public static partial class CreateAnalyticalModelSettingsCommandMapper
{
    public static partial AnalyticalModelSettings ToDomainObject(
        this AnalyticalModelSettingsCommand command
    );
}
