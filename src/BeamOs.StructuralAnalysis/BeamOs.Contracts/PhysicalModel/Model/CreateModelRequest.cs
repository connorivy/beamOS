using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.PhysicalModel.Model;

public record CreateModelRequest(
    string Name,
    string Description,
    PhysicalModelSettingsDto Settings,
    string? Id = null
);

public record PhysicalModelSettingsDto(UnitSettingsDtoVerbose UnitSettings);
