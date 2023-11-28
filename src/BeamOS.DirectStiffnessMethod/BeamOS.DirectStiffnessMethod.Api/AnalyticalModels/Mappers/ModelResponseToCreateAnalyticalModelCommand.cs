using BeamOS.Common.Api.Interfaces;
using BeamOS.DirectStiffnessMethod.Api.AnalyticalElement1ds.Mappers;
using BeamOS.DirectStiffnessMethod.Api.AnalyticalNodes.Mappers;
using BeamOS.DirectStiffnessMethod.Api.Materials.Mappers;
using BeamOS.DirectStiffnessMethod.Api.SectionProfiles.Mappers;
using BeamOS.DirectStiffnessMethod.Application.AnalyticalModels.Commands;
using BeamOS.PhysicalModel.Contracts.Model;

namespace BeamOS.DirectStiffnessMethod.Api.AnalyticalModels.Mappers;

public class ModelResponseToCreateAnalyticalModelCommand(
    ModelSettingsResponseMapper modelSettingsResponseMapper,
    ModelResponseToCreateAnalyticalNodeCommands modelResponseToCreateAnalyticalNodeCommandsMapper,
    Element1dResponseToCreateAnalyticalElement1dCommandMapper element1dResponseToCreateAnalyticalElement1dCommandMapper,
    MaterialResponseToCreateMaterialCommand materialResponseToCreateMaterialCommand,
    SectionProfileResponseToCreateSectionProfileCommand sectionProfileResponseToCreateSectionProfileCommand
    )
    : IMapper<ModelResponse, CreateAnalyticalModelFromPhysicalModelCommand>
{
    public CreateAnalyticalModelFromPhysicalModelCommand Map(ModelResponse source)
    {
        return new CreateAnalyticalModelFromPhysicalModelCommand(
            source.Id,
            source.Name,
            source.Description,
            modelSettingsResponseMapper.Map(source.Settings),
            modelResponseToCreateAnalyticalNodeCommandsMapper.Map(source),
            element1dResponseToCreateAnalyticalElement1dCommandMapper.Map(source.Element1Ds).ToList(),
            materialResponseToCreateMaterialCommand.Map(source.Materials).ToList(),
            sectionProfileResponseToCreateSectionProfileCommand.Map(source.SectionProfiles).ToList()
            );
    }
}
