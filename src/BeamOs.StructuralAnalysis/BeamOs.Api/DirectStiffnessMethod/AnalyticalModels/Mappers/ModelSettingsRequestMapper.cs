using BeamOs.Api.Common.Interfaces;
using BeamOs.Application.DirectStiffnessMethod.AnalyticalModels.Commands;
using BeamOs.Contracts.PhysicalModel.Model;
using Riok.Mapperly.Abstractions;

namespace BeamOS.DirectStiffnessMethod.Api.AnalyticalModels.Mappers;

[Mapper]
public partial class ModelSettingsResponseMapper
    : IMapper<ModelSettingsResponse, ModelSettingsCommand>
{
    public ModelSettingsCommand Map(ModelSettingsResponse source) => this.ToCommand(source);

    public partial ModelSettingsCommand ToCommand(ModelSettingsResponse source);
}
