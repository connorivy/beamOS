using BeamOs.Application.Common.Commands;
using BeamOs.Application.DirectStiffnessMethod.AnalyticalElement1ds.Commands;
using BeamOs.Application.DirectStiffnessMethod.AnalyticalNodes.Commands;
using BeamOs.Application.DirectStiffnessMethod.Materials;
using BeamOs.Application.DirectStiffnessMethod.SectionProfiles;

namespace BeamOs.Application.DirectStiffnessMethod.AnalyticalModels.Commands;

public record CreateAnalyticalModelFromPhysicalModelCommand(
    string Id,
    string Name,
    string Description,
    AnalyticalModelSettingsCommand Settings,
    List<CreateAnalyticalNodeCommand> Nodes,
    List<CreateAnalyticalElement1dCommand> Element1Ds,
    List<CreateMaterialCommand> Materials,
    List<CreateSectionProfileCommand> SectionProfiles
);

public record AnalyticalModelSettingsCommand(UnitSettingsCommand UnitSettings);
