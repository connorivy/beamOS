using BeamOs.Application.Common.Commands;
using UnitsNet.Units;

namespace BeamOs.Application.PhysicalModel.Models.Commands;

public record CreateModelCommand(
    string Name,
    string Description,
    PhysicalModelSettingsCommand Settings
);

public record PhysicalModelSettingsCommand(UnitSettingsCommand UnitSettings);
