using BeamOs.Application.DirectStiffnessMethod.AnalyticalElement1ds.Commands;
using BeamOs.Application.DirectStiffnessMethod.AnalyticalNodes.Commands;
using BeamOs.Application.DirectStiffnessMethod.Materials;
using BeamOs.Application.DirectStiffnessMethod.SectionProfiles;

namespace BeamOs.Application.DirectStiffnessMethod.AnalyticalModels.Commands;

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
