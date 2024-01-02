using BeamOS.DirectStiffnessMethod.Application.AnalyticalElement1ds.Commands;
using BeamOS.DirectStiffnessMethod.Application.AnalyticalNodes.Commands;
using BeamOS.DirectStiffnessMethod.Application.Materials;
using BeamOS.DirectStiffnessMethod.Application.SectionProfiles;

namespace BeamOS.DirectStiffnessMethod.Application.AnalyticalModels.Commands;

public record CreateAnalyticalModelFromPhysicalModelCommand(
    string Id,
    string Name,
    string Description,
    ModelSettingsCommand Settings,
    List<CreateAnalyticalNodeCommand> Nodes,
    List<CreateAnalyticalElement1dCommand> Element1Ds,
    List<CreateMaterialCommand> Materials,
    List<CreateSectionProfileCommand> SectionProfiles
);

public record ModelSettingsCommand(UnitSettingsCommand UnitSettings);

public record UnitSettingsCommand(
    string LengthUnit,
    string AreaUnit,
    string VolumeUnit,
    string ForceUnit,
    string ForcePerLengthUnit,
    string TorqueUnit
);
