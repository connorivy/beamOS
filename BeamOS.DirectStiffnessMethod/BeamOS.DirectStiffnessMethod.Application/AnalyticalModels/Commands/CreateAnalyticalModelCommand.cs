using BeamOS.DirectStiffnessMethod.Application.AnalyticalElement1ds.Commands;
using BeamOS.DirectStiffnessMethod.Application.AnalyticalNodes.Commands;

namespace BeamOS.DirectStiffnessMethod.Application.AnalyticalModels.Commands;

public record CreateAnalyticalModelCommand(
    string Id,
    string Name,
    string Description,
    ModelSettingsCommand Settings,
    List<CreateAnalyticalNodeCommand>? Nodes = null,
    List<CreateAnalyticalElement1dCommand>? Element1Ds = null,
    List<MaterialResponse>? Materials = null,
    List<SectionProfileResponse>? SectionProfiles = null);

public record ModelSettingsCommand(
    UnitSettingsCommand UnitSettings);

public record UnitSettingsCommand(
    string LengthUnit,
    string AreaUnit,
    string VolumeUnit,
    string ForceUnit,
    string ForcePerLengthUnit,
    string TorqueUnit);
