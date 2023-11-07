using BeamOS.PhysicalModel.Domain.AnalyticalModelAggregate;
using BeamOS.PhysicalModel.Domain.AnalyticalModelAggregate.ValueObjects;
using FastEndpoints;

namespace BeamOS.PhysicalModel.Application.Models.Commands;
public class CreateModelCommandHandler : ICommandHandler<CreateModelCommand, AnalyticalModel>
{
    public async Task<AnalyticalModel> ExecuteAsync(CreateModelCommand command, CancellationToken ct)
    {
        await Task.CompletedTask;

        UnitSettings unitSettings = new(
            command.Settings.UnitSettings.LengthUnit,
            command.Settings.UnitSettings.AreaUnit,
            command.Settings.UnitSettings.VolumeUnit,
            command.Settings.UnitSettings.ForceUnit,
            command.Settings.UnitSettings.ForcePerLengthUnit,
            command.Settings.UnitSettings.TorqueUnit
            );
        AnalyticalModelSettings settings = new(unitSettings);

        var model = AnalyticalModel.Create(command.Name, command.Description, settings);

        // TODO : persist model

        return model;
    }
}
