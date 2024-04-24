using BeamOs.Api.DirectStiffnessMethod.AnalyticalElement1ds.Mappers;
using BeamOs.Api.DirectStiffnessMethod.AnalyticalNodes.Mappers;
using BeamOs.Api.DirectStiffnessMethod.SectionProfiles.Mappers;
using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.DirectStiffnessMethod.AnalyticalModels.Commands;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOS.DirectStiffnessMethod.Api.AnalyticalModels.Mappers;
using BeamOS.DirectStiffnessMethod.Api.Materials.Mappers;

namespace BeamOs.Api.DirectStiffnessMethod.AnalyticalModels.Mappers;

public class ModelResponseHydratedToCreateAnalyticalModelCommand(
    ModelSettingsResponseMapper modelSettingsResponseMapper,
    ModelResponseHydratedToCreateAnalyticalNodeCommands modelResponseToCreateAnalyticalNodeCommandsMapper,
    Element1dResponseToCreateAnalyticalElement1dCommandMapper element1dResponseToCreateAnalyticalElement1dCommandMapper,
    MaterialResponseToCreateMaterialCommand materialResponseToCreateMaterialCommand,
    SectionProfileResponseToCreateSectionProfileCommand sectionProfileResponseToCreateSectionProfileCommand
) : IMapper<ModelResponseHydrated, CreateAnalyticalModelFromPhysicalModelCommand>
{
    public CreateAnalyticalModelFromPhysicalModelCommand Map(ModelResponseHydrated source)
    {
        return new CreateAnalyticalModelFromPhysicalModelCommand(
            source.Id,
            source.Name,
            source.Description,
            modelSettingsResponseMapper.Map(source.Settings),
            modelResponseToCreateAnalyticalNodeCommandsMapper.Map(source),
            element1dResponseToCreateAnalyticalElement1dCommandMapper
                .Map(source.Element1Ds)
                .ToList(),
            materialResponseToCreateMaterialCommand.Map(source.Materials).ToList(),
            sectionProfileResponseToCreateSectionProfileCommand.Map(source.SectionProfiles).ToList()
        );
    }
}
