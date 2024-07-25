using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.PhysicalModel.Model;

public record CreateModelRequest(
    string Name,
    string Description,
    PhysicalModelSettings Settings,
    string? Id = null
);

public record PhysicalModelSettings(UnitSettingsDtoVerbose UnitSettings, bool YAxisUp = true);
