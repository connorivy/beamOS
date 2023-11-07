using BeamOS.PhysicalModel.Domain.AnalyticalModelAggregate;
using FastEndpoints;
using UnitsNet.Units;

namespace BeamOS.PhysicalModel.Application.Models.Commands;

public record CreateModelCommand(
    string Name,
    string Description,
    AnalyticalModelSettingsCommand Settings) : ICommand<AnalyticalModel>;

public record AnalyticalModelSettingsCommand(
    UnitSettingsCommand UnitSettings);

public record UnitSettingsCommand(
    LengthUnit LengthUnit,
    AreaUnit AreaUnit,
    VolumeUnit VolumeUnit,
    ForceUnit ForceUnit,
    ForcePerLengthUnit ForcePerLengthUnit,
    TorqueUnit TorqueUnit);
