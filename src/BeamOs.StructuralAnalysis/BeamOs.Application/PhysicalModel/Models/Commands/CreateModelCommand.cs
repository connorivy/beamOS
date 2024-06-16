using BeamOs.Application.Common.Commands;

namespace BeamOs.Application.PhysicalModel.Models.Commands;

public record CreateModelCommand(
    string Name,
    string Description,
    PhysicalModelSettingsCommand Settings,
    string? Id = null
);

public record PhysicalModelSettingsCommand(UnitSettingsCommand UnitSettings);
